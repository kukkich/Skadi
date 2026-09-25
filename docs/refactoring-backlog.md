# Skadi — бэклог рефакторинга

Выжимка ревью для работы в отдельных сессиях. Каждая задача самодостаточна: возьми по ID
(«сделай B1 из docs/refactoring-backlog.md»), не перечитывая весь проект.
Все пункты сверены с кодом `master` после PR #21. Выполненную задачу — удалить из файла.

## Контекст (прочитать один раз)

- **Что это:** библиотека МКЭ на .NET 9 / C# 13 для 2D электромагнитных (гармонических, комплексных,
  рёберных H(curl)) краевых задач. Цель ветки — обратная задача (`Skadi/Algorithms/InverseTask/` пока пуста),
  поэтому прямая задача будет гоняться в цикле: корректность и цена одного прогона важны.
- **Слои:** `LinearAlgebra` → `FEM/Core` + `FEM/Assembling` → `EquationsSystem` (решатели, предобуславливатели)
  → `Algorithms` (Гаусс, сплайны) → `Geometry` / `Expressions` / `IO`.
- **Линейная алгебра (уже отрефакторена, PR #21):** `LinAl` удалён. V-V / V-scalar — `VectorOps`,
  плотные M-scalar / M-M / M-V — `MatrixOps`, проверки размеров — `Shape` (с `CallerArgumentExpression`).
  M-V для разреженных форматов — `ILinearOperator.MultiplyOn` на самих `CSRMatrix`, `SparseMatrix`,
  `SymmetricRowSparseMatrix`. Kernel-методы принимают `Span`, ничего не аллоцируют и перезаписывают
  `destination` (`=`, не `+=`). Convenience-методы выделяют память, если `destination` не передан.
- **Тесты:** NUnit, `dotnet test Skadi.Tests/Skadi.Tests.csproj` (~3 мин). 11 тестов в skip ещё до
  рефакторинга — это норма.
- **Конвенции:** file-scoped namespaces, primary constructors, комментарии только для неочевидного,
  без обратной совместимости (вызовы обновлять, а не оставлять шимы).

## P0 — баги (корректность)

**B1. Неверная ветка корня в обратном отображении шаблонного элемента**
`(originKsi2, originKsi2)` вместо `(originKsi2, originEta2)`:
`Skadi/FEM/2D/Solution/QuadLinearSolution.cs:135`, `ImpedanceSolution.cs:89`, `HarmonicQuadLinearSolution.cs:85`.
Тихо портит значение решения в точке для непрямоугольных элементов, когда подходит второй корень
квадратного уравнения. Нужен тест на непрямоугольный четырёхугольник, где срабатывает вторая ветка.
Лучше делать вместе с D1 — тогда фикс будет в одном месте.

**B2. `EdgeResolver.GetElementsByEdgeId` проверяет начало ребра дважды**
`Skadi/FEM/Assembling/Edges/EdgeResolver.cs:111`: `Contains(edge.Begin) && Contains(edge.Begin)` →
второй должен быть `edge.End`. Метод не покрыт тестами — добавить.

**B3. LDLT-разложение оборачивается в Холецкого**
`Skadi/EquationsSystem/Preconditions/LDLT/IncompleteLDLTPreconditionerFactory.cs:11` создаёт
`CholeskyPreconditioner`, а должен `LDLTPreconditioner` (сейчас он мёртвый). Холецкий делит на диагональ
в обоих ходах, так что к LDLT-разложению применяется не тот оператор. CG сходится, но предобуславливатель
неправильный. Проверить тест фабрики: возможно, у него ослаблен допуск, чтобы скрыть проблему.

**B4. `Vector2D.IsNormal` сравнивает double через `==`**
`Skadi/Geometry/2D/Vector2D.cs` — `value.Length == 1`. Сравнивать с допуском.

## P1 — дизайн (дублирование и абстракции)

**D1. Билинейное отображение шаблонного элемента скопировано в 5+ местах**
Коэффициенты `b1..b6`, `alpha0..2`, `GetJacobian` и обратное отображение `(x,y)→(ξ,η)` + `IsPointInTriangle`:
`FEM/2D/Assembling/QuadLinearAssembler2D.cs`, `HarmonicQuadLinearAssembler.cs`,
`FEM/2D/Solution/QuadLinearSolution.cs`, `HarmonicQuadLinearSolution.cs`, `ImpedanceSolution.cs`
(в коде есть `// Todo дублируется`). Копии уже разошлись (см. B1). Вынести в один тип (например,
`QuadElementMapping`: прямое/обратное отображение, якобиан). Сначала посмотреть заготовку
`FEM/Core/Geometry/2D/Quad/ITemplatePointsMapper` / `LinearTemplatePointsMapper`: сборщики её не используют.

**D2. `QuadLinearAssembler2D` и `HarmonicQuadLinearAssembler` совпадают на ~90%**
Отличаются удвоением DOF (перестановка `2i`, `2i+1`) и блоком 2×2 с `Sigma*Omega`. Общий интеграл
жёсткости/массы вынести (после D1).

**D3. Построители портрета матрицы**
`FEM/Assembling/PortraitBuilders/{SymmetricMatrixPortraitBuilder, SymmetricMatrixEdgeGridPortraitBuilder,
HarmonicMatrixPortraitBuilder}.cs`, `FEM/Assembling/Edges/EdgesPortraitBuilder.cs`. Похоже, у них общий
алгоритм (adjacency → отсортированные множества → массивы). Проверить и вынести общую часть,
оставив различия в раскладке (симметричная / гармоническая / рёберная).

**D4. Boundary appliers дублируют if/else-цепочку и перекомпилируют выражения**
`FEM/Assembling/Boundary/RegularGrid/{RegularBoundaryApplier, Harmonic/HarmonicRegularBoundaryApplier,
Vectors/VectorRegularBoundaryApplier}.cs` — одинаковая диспетчеризация по `BoundaryConditionType`,
отличается только тип значения. `expression.Compile()` на каждый вызов `Apply`, то есть на каждый
прогон прямой задачи. Кешировать скомпилированные делегаты по `ExpressionId`.

**D5. Решатели СЛАУ — несогласованные контракты**
- `EquationsSystem/Preconditions/IPreconditioner.cs`: `IPreconditioner` (умножение, `: ILinearOperator`)
  и `IPreconditioner<TResult>` (`Decompose`) — разные роли под одним именем. Второй переименовать
  (например, `IDecomposer<TResult>`).
- `BiCGStabSolver<T> where T : ILinearOperator` обобщён, а `ConjugateGradientSolver` жёстко привязан
  к `SymmetricRowSparseMatrix` и хранит итерационное состояние в полях (не реентерабелен). Привести к одному стилю.
- `ConjugateGradientSolver.cs:77` — `Console.WriteLine` при внедрённом `ILogger`.
- Решатели не сообщают, сошлись ли они и за сколько итераций: вернуть результат с этой информацией или хотя бы логировать.

**D6. Прямой и обратный ход LU продублированы**
`EquationsSystem/Solver/SparsePartialLUResolver.cs` (`CalcY`/`CalcX`) и `LUProfile.cs`.
`LUSparseThroughProfileConversion` создаёт `new LUProfile()` внутри метода, а не получает через конструктор.

**D7. Поиск элемента по точке — линейный `.First(ElementHas)` в 7 местах**
`FEM/2D/Solution/{QuadLinearSolution(×2), HarmonicQuadLinearSolution, ImpedanceSolution, VectorFEMSolution2D}.cs`,
`Algorithms/Splines/{SplineEquationAssembler, 1D/CubicLagrange/LagrangeSpline, 2D/Smooth/SmoothingSpline}.cs`.
`ElementHas` тоже скопирован. Вынести в один сервис локации (например, `IElementLocator` над `Grid`);
потом его можно ускорить пространственным индексом, не трогая вызывающий код. O(n) на точку.

**D8. `SparseMatrix[i, j]` возвращает индекс, а не значение**
`LinearAlgebra/Matrices/Sparse/SparseMatrix.cs` — индексатор ведёт себя не как у соседних матриц.
Переименовать в явный метод (`IndexOf(row, column)`); `Array.IndexOf` O(n) заменить на бинарный поиск,
если столбцы в строке отсортированы (проверить построители портрета). Вызывается при сборке
на каждый локальный элемент.

**D9. `MatrixSpan` и `ReadOnlyMatrixSpan` разъехались по API**
`ReadOnlyMatrixSpan` уже имеет `Rows`/`Columns`/`IsSquare`, а `MatrixSpan` — только `Size`.
Выровнять и потом упростить проверки в `MatrixOps` (сейчас там `destination.Size` передаётся за строки и столбцы).

**D10. Дубли `Nullify` для `Vector`**
У `Vector` есть и instance-метод `Nullify()`, и extension `Extensions.Nullify(this Vector)`
(`LinearAlgebra/Vectors/Extensions.cs`) — extension удалить.

**D11. `Method<TConfig>` — наследование ради двух полей**
`Skadi/Method.cs` (там есть `//Todo replace with composition`). Заменить на внедрение `Config`/`ILogger` в конструктор.

**D12. Зависимости**
`Skadi/Skadi.csproj`: `System.Linq.Dynamic.Core 1.4.9` — предупреждение NU1903 (известная уязвимость
высокой критичности), обновить. `Microsoft.CodeAnalysis.CSharp.Scripting` нигде не используется — удалить.

## P2 — мелочи и незавершённое

- **N1. Опечатки в публичных именах:** `Hollesky` → `Cholesky` (namespace `EquationsSystem/Preconditions/Hollesky`,
  `IncompleteHollesky.cs`, `IncompleteHolleskyPreconditionerFactory.cs`, тесты), `FEM/Core/Geometry/2D/Oreintation.cs`,
  `Skadi.Tests/LinAlTests/MVOperations/CSRMastrixOperationsTests.cs`, `edgesCunt` в
  `SymmetricMatrixEdgeGridPortraitBuilder.cs`. Разный регистр: `IReadOnlyMatrix` vs `IReadonlyVector`.
- **N2. `BoundTypes2D`** (`Geometry/2D/BoundTypes2D.cs`) — int-константы; рядом уже есть enum `CurveType2D`.
- **N3. Заглушки `NotImplementedException`** — решить: доделать или удалить.
  Hermite (`FEM/2D/Assembling/HermiteLocalAssembler.cs`, `FEM/2D/BasisFunctions/HermiteBasisFunctions2DProvider.cs`),
  `FEM/1D/Assembling/LagrangeCubicAssembler1D.AssembleRightSide`, третье краевое во всех `*RegularBoundaryApplier`,
  `VectorSecondConditionApplier` (после `throw` мёртвый код), `VectorRegularBoundaryApplier`.
- **N4. `Geometry/3D/Vector3D.cs`** — пустой `record struct`. При появлении 3D не копировать `Vector2D`, а обобщить.
- **N5. Сплайны:** `SmoothingSplineCreator.CalculateWeights` всегда возвращает единицы;
  `SplineContext.Beta` не используется — недописанная регуляризация (важно для обратной задачи).
- **N6. Исключения:** голый `throw new Exception(...)` (`LUPreconditionerCSR`, `MaterialReader` и др.)
  заменить на конкретные типы; сообщения на одном языке.
- **N7. `IO/LinAlIO.cs`** умеет только плотные `Matrix`/`Vector` и пишет в `Console`.
- **N8. `Skadi.Tests/UnitTest1.cs`** — шаблонный `Assert.Pass()`, удалить.

## Покрытие тестами (добавлять вместе с соответствующими задачами)

- `Geometry` — тестов нет совсем, хотя сборка МКЭ на нём держится.
- Нет тестов: `HarmonicQuadLinearAssembler`, классы `*Solution` (из-за этого жил B1), `EdgeResolver.GetElementsByEdgeId` (B2),
  `GaussExcluderSparse`/`GaussExcluderSymmetricSparse`, `GaussZeidelSolver`, `SmoothingSpline*`,
  `LUPreconditionerCSR*`, `DiagonalPreconditioner`, `IncompleteLDLTPreconditionerFactory` (B3).

## Проверено и НЕ является проблемой

- `GaussExcluderSparse` не переносит значение в правую часть других строк — это корректно: он заменяет
  строку целиком (диагональ 1, правая часть = значение), столбец не трогает. Для несимметричных решателей
  так можно. Перенос в правую часть нужен только `GaussExcluderSymmetricSparse`, чтобы сохранить симметрию.

## Рекомендуемый порядок

B2, B3, B4 (быстрые) → D1 + B1 → D2 → D7 → D4 → D12 → остальное P1 → P2.
