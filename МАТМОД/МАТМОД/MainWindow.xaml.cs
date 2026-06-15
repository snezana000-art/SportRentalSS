using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MaxFlowWPFApp
{
    public partial class MainWindow : Window
    {
        private MaxFlowSolver solver;
        private List<Edge> edges;

        public MainWindow()
        {
            InitializeComponent();
            edges = new List<Edge>();
            UpdateEdgeList();
        }

        private void BtnAddEdge_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int from = int.Parse(txtFrom.Text);
                int to = int.Parse(txtTo.Text);
                int capacity = int.Parse(txtCapacity.Text);

                if (from == to)
                {
                    MessageBox.Show("Начальная и конечная вершины не могут совпадать!");
                    return;
                }

                edges.Add(new Edge { From = from, To = to, Capacity = capacity });
                UpdateEdgeList();
                ClearResults();
            }
            catch (FormatException)
            {
                MessageBox.Show("Пожалуйста, введите корректные числовые значения!");
            }
        }

        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int vertices = int.Parse(txtVertices.Text);
                int source = int.Parse(txtSource.Text);
                int sink = int.Parse(txtSink.Text);

                if (source < 0 || source >= vertices || sink < 0 || sink >= vertices)
                {
                    MessageBox.Show("Номера истока и стока должны быть в диапазоне [0, количество вершин-1]!");
                    return;
                }

                if (source == sink)
                {
                    MessageBox.Show("Исток и сток не могут совпадать!");
                    return;
                }

                // Создаём решатель и добавляем рёбра
                solver = new MaxFlowSolver(vertices);
                foreach (var edge in edges)
                {
                    solver.AddEdge(edge.From, edge.To, edge.Capacity);
                }

                // Вычисляем максимальный поток
                int maxFlow = solver.FindMaxFlow(source, sink);

                // Отображаем результаты
                DisplayResults(maxFlow, source, sink);
                DisplayCapacityMatrix(vertices);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            edges.Clear();
            UpdateEdgeList();
            ClearResults();
        }

        private void BtnLoadExample_Click(object sender, RoutedEventArgs e)
        {
            // Загружаем пример графа
            edges.Clear();

            // Пример графа с 6 вершинами
            edges.Add(new Edge { From = 0, To = 1, Capacity = 16 });
            edges.Add(new Edge { From = 0, To = 2, Capacity = 13 });
            edges.Add(new Edge { From = 1, To = 2, Capacity = 10 });
            edges.Add(new Edge { From = 1, To = 3, Capacity = 12 });
            edges.Add(new Edge { From = 2, To = 1, Capacity = 4 });
            edges.Add(new Edge { From = 2, To = 4, Capacity = 14 });
            edges.Add(new Edge { From = 3, To = 2, Capacity = 9 });
            edges.Add(new Edge { From = 3, To = 5, Capacity = 20 });
            edges.Add(new Edge { From = 4, To = 3, Capacity = 7 });
            edges.Add(new Edge { From = 4, To = 5, Capacity = 4 });

            txtVertices.Text = "6";
            txtSource.Text = "0";
            txtSink.Text = "5";

            UpdateEdgeList();
            ClearResults();
        }

        private void UpdateEdgeList()
        {
            lstEdges.Items.Clear();
            foreach (var edge in edges)
            {
                lstEdges.Items.Add($"Ребро {edge.From} → {edge.To}: проп. способность = {edge.Capacity}");
            }
        }

        private void DisplayResults(int maxFlow, int source, int sink)
        {
            txtResult.Text = $"МАКСИМАЛЬНЫЙ ПОТОК: {maxFlow}\n\n";
            txtResult.Text += $"Из истока {source} в сток {sink}\n";
            txtResult.Text += $"Количество вершин: {solver.VerticesCount}\n";
            txtResult.Text += $"Количество рёбер: {edges.Count}\n\n";

            txtFlowDetails.Text = "РАСПРЕДЕЛЕНИЕ ПОТОКОВ:\n\n";
            var flowDistribution = solver.GetFlowDistribution();
            foreach (var flow in flowDistribution)
            {
                if (flow.Flow > 0)
                {
                    txtFlowDetails.Text += $"Ребро {flow.From} → {flow.To}: {flow.Flow}/{flow.Capacity}\n";
                }
            }
        }

        private void DisplayCapacityMatrix(int vertices)
        {
            matrixPanel.Children.Clear();

            var grid = new Grid();
            grid.Margin = new Thickness(5);

            // Создаём колонки и строки для матрицы
            for (int i = 0; i <= vertices; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40) });
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            }

            // Заголовки строк и столбцов
            for (int i = 0; i < vertices; i++)
            {
                // Заголовки строк
                var rowHeader = new TextBlock
                {
                    Text = i.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = FontWeights.Bold,
                    Background = Brushes.LightGray
                };
                Grid.SetRow(rowHeader, i + 1);
                Grid.SetColumn(rowHeader, 0);
                grid.Children.Add(rowHeader);

                // Заголовки столбцов
                var colHeader = new TextBlock
                {
                    Text = i.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = FontWeights.Bold,
                    Background = Brushes.LightGray
                };
                Grid.SetRow(colHeader, 0);
                Grid.SetColumn(colHeader, i + 1);
                grid.Children.Add(colHeader);
            }

            // Заполняем матрицу
            for (int i = 0; i < vertices; i++)
            {
                for (int j = 0; j < vertices; j++)
                {
                    var textBlock = new TextBlock
                    {
                        Text = solver.GetCapacity(i, j).ToString(),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    if (solver.GetCapacity(i, j) > 0)
                    {
                        textBlock.Background = Brushes.LightGreen;
                        textBlock.FontWeight = FontWeights.Bold;
                    }

                    Grid.SetRow(textBlock, i + 1);
                    Grid.SetColumn(textBlock, j + 1);
                    grid.Children.Add(textBlock);
                }
            }

            matrixPanel.Children.Add(grid);
        }

        private void ClearResults()
        {
            txtResult.Text = "";
            txtFlowDetails.Text = "";
            matrixPanel.Children.Clear();
        }
    }

    // Классы для работы с графом и потоком
    public class Edge
    {
        public int From { get; set; }
        public int To { get; set; }
        public int Capacity { get; set; }
    }

    public class FlowInfo
    {
        public int From { get; set; }
        public int To { get; set; }
        public int Flow { get; set; }
        public int Capacity { get; set; }
    }

    public class MaxFlowSolver
    {
        private int verticesCount;
        private int[,] capacity;
        private int[,] residualCapacity;

        public int VerticesCount => verticesCount;

        public MaxFlowSolver(int vertices)
        {
            verticesCount = vertices;
            capacity = new int[vertices, vertices];
            residualCapacity = new int[vertices, vertices];
        }

        public void AddEdge(int from, int to, int cap)
        {
            if (from < 0 || from >= verticesCount || to < 0 || to >= verticesCount)
                throw new ArgumentException("Неверные номера вершин");

            capacity[from, to] = cap;
            residualCapacity[from, to] = cap;
        }

        public int GetCapacity(int from, int to)
        {
            return capacity[from, to];
        }

        private bool BFS(int source, int sink, int[] parent)
        {
            bool[] visited = new bool[verticesCount];
            System.Collections.Generic.Queue<int> queue = new System.Collections.Generic.Queue<int>();

            queue.Enqueue(source);
            visited[source] = true;
            parent[source] = -1;

            while (queue.Count > 0)
            {
                int current = queue.Dequeue();

                for (int next = 0; next < verticesCount; next++)
                {
                    if (!visited[next] && residualCapacity[current, next] > 0)
                    {
                        queue.Enqueue(next);
                        parent[next] = current;
                        visited[next] = true;

                        if (next == sink)
                            return true;
                    }
                }
            }

            return false;
        }

        public int FindMaxFlow(int source, int sink)
        {
            if (source == sink)
                throw new ArgumentException("Исток и сток не могут совпадать");

            int[] parent = new int[verticesCount];
            int maxFlow = 0;

            // Копируем исходные пропускные способности в остаточную сеть
            Array.Copy(capacity, residualCapacity, capacity.Length);

            // Пока существует увеличивающий путь
            while (BFS(source, sink, parent))
            {
                // Находим минимальную пропускную способность на пути
                int pathFlow = int.MaxValue;
                for (int v = sink; v != source; v = parent[v])
                {
                    int u = parent[v];
                    pathFlow = Math.Min(pathFlow, residualCapacity[u, v]);
                }

                // Обновляем остаточные пропускные способности
                for (int v = sink; v != source; v = parent[v])
                {
                    int u = parent[v];
                    residualCapacity[u, v] -= pathFlow;
                    residualCapacity[v, u] += pathFlow;
                }

                maxFlow += pathFlow;
            }

            return maxFlow;
        }

        public List<FlowInfo> GetFlowDistribution()
        {
            var flowDistribution = new List<FlowInfo>();

            for (int i = 0; i < verticesCount; i++)
            {
                for (int j = 0; j < verticesCount; j++)
                {
                    if (capacity[i, j] > 0)
                    {
                        int flow = capacity[i, j] - residualCapacity[i, j];
                        if (flow > 0)
                        {
                            flowDistribution.Add(new FlowInfo
                            {
                                From = i,
                                To = j,
                                Flow = flow,
                                Capacity = capacity[i, j]
                            });
                        }
                    }
                }
            }

            return flowDistribution;
        }
    }
}