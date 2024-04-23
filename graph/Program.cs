using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

/*
 * Задание 2
3 Реализовать статическую функцию, которая позволяет. Построить граф, являющийся объединением двух заданных. 
Результатом должен быть объект графа.
 
4 Реализовать статическую функцию, которая позволяет. Построить граф, являющийся соединением данного графа с другим. 
Результатом должен быть объект графа. 
 */

namespace graph
{
    class GraphInterface
    {
        public Graph graph;
        string path;
        public GraphInterface(Graph graph, string path)
        {
            this.graph = graph;
            this.path = path;
        }

        public bool Operation(bool continueOp)
        {
            int i;
            Console.WriteLine("Выберите операцию:\n1 - добавить вершину" +
                "\n2 - удалить вершину\n3 - создать ребро" +
                "\n4 - добавить вес ребру\n5 - удалить ребро\n6 - напечатать список смежности графа" +
                "\n7-напечатать список рёбер\n8 - завершить работу с графом");
            i = int.Parse(Console.ReadLine());
            switch (i)
            {
                case 1:
                    {
                        Console.Write("Введите идентификатор вершины: ");
                        if (graph.AddVertex(int.Parse(Console.ReadLine())))
                            Console.WriteLine("Операция успешна");
                        else Console.WriteLine("Неверный id");
                    }
                    return continueOp;
                case 2:
                    {
                        Console.Write("Введите идентификатор вершины: ");
                        if (graph.DeleteVertex(int.Parse(Console.ReadLine())))
                            Console.WriteLine("Операция успешна");
                        else Console.WriteLine("Неверный id");
                    }
                    return continueOp;
                case 3:
                    {
                        int v1, v2;
                        Console.Write("Введите идентификатор вершины, из которой исходит ребро: ");
                        v1 = int.Parse(Console.ReadLine());
                        Console.Write("Введите идентификатор вершины, в которую входит ребро: ");
                        v2 = int.Parse(Console.ReadLine());
                        if(graph.CreateEdge(v1, v2))
                            Console.WriteLine("Операция успешна");
                        else Console.WriteLine("Неверный id");
                    }
                    return continueOp;
                case 4:
                    {
                        int ID, weight;
                        Console.Write("Введите идентификатор ребра: ");
                        ID = int.Parse(Console.ReadLine());
                        Console.Write("Введите вес: ");
                        weight = int.Parse(Console.ReadLine());
                        if (graph.AddEdgeWeight(ID, weight))
                            Console.WriteLine("Операция успешна");
                        else Console.WriteLine("Неверный id");
                    }
                    return continueOp;
                case 5:
                    {
                        int ID;
                        Console.Write("Введите идентификатор ребра: ");
                        ID = int.Parse(Console.ReadLine());
                        if (graph.DeleteEdge(ID))
                            Console.WriteLine("Операция успешна");
                        else Console.WriteLine("Неверный id");
                    }
                    return continueOp;
                case 6:
                    {
                        FilePrintAdjacencyList();
                    }
                    return continueOp;
                case 7:
                    {
                        PrintEdgeList();
                    }
                    return continueOp;
                case 8:
                    {
                        Console.WriteLine("Работа завершена");
                        return !continueOp;
                    }
                default: return continueOp;

            }
        }
        public void PrintEdgeList()
        {
            foreach (var vertex in graph.vertices)
            {
                foreach (Edge edge in vertex.Value)
                {
                    Console.WriteLine($"источник:{vertex.Key}," +
                        $" вхождение:{edge.ConnectedVertex}, вес:{edge.Weight}," +
                        $" ID: {edge.Id}, pairId: {edge.PairId}");
                }
            }
        }
        
        public void FilePrintAdjacencyList()
        {
            string output = "";
            StreamWriter fileOut = new StreamWriter(path, false);
            if (graph.isOriented)
                output += "True\n";
            else output += "False\n";
            if (graph.isMultigraph)
                output += "True\n";
            else output += "False\n";
            foreach (var vertex in graph.vertices)
            {
                output += $"{vertex.Key} ";
                foreach (Edge edge in vertex.Value)
                {
                    output += $"{edge.ConnectedVertex}_";
                }
                output += "_";
                output += "\n";
            }
            fileOut.Write($"{output}");
            fileOut.Close();
        }
    }

    class Edge
    {
        public int Weight;
        public int ConnectedVertex;
        static int CurrentId = 1;
        public int Id;
        public int PairId;

        public Edge()
        {
            Weight = 0;
            ConnectedVertex = 0;//id начинаются с 1, число 0 означает изолированную вершину
            Id = CurrentId;
            CurrentId++;
            PairId = 0;//т.е. нет пары
        }

        public Edge(int ConnectedVertex = 0, int PairId = 0, int Weight = 0)
        {
            this.Weight = Weight;
            this.ConnectedVertex = ConnectedVertex;
            this.Id = CurrentId;
            CurrentId++;
            if (PairId != 0)
            {
                this.PairId = PairId;
            }
            else this.PairId = 0;
        }
        public Edge(Edge edge)
        {
            this.Weight = edge.Weight;
            this.ConnectedVertex = edge.ConnectedVertex;
            this.Id = CurrentId;
            CurrentId++;
            this.PairId = edge.PairId;
        }
    }
    class Graph
    {
        public Dictionary<int, List<Edge>> vertices;//идентификатор вершины, смежная вершина и вес
        public bool isOriented;
        public bool isMultigraph;
        static int CurrentPairId = 1;
        public Graph()
        {
            vertices = new Dictionary<int, List<Edge>>();
            isOriented = false;
            isMultigraph = false;
        }
        public Graph(bool isOriented, bool isMultigraph,
           Dictionary<int, List<Edge>> vertices)//если указаны все параметры
        {
            this.isOriented = isOriented;
            this.vertices = vertices;
            this.isMultigraph = isMultigraph;
        }

        public Graph(Graph CopyGraph)
        {
            vertices = new Dictionary<int, List<Edge>>();
            foreach (var vertex in CopyGraph.vertices)
            {
                vertices.Add(vertex.Key, new List<Edge>());
                foreach (Edge edge in vertex.Value)
                {
                    vertices[vertex.Key].Add(new Edge(edge));
                }
            }
            isOriented = CopyGraph.isOriented;
            isMultigraph = CopyGraph.isMultigraph;
        }
        public Graph(string path)
        {
            vertices = new Dictionary<int, List<Edge>>();
            string[] tempArr;
            StreamReader fileIn = new StreamReader(path);
            isOriented = Convert.ToBoolean(fileIn.ReadLine());
            isMultigraph = Convert.ToBoolean(fileIn.ReadLine());
            while (fileIn.Peek() != -1)
            {
                tempArr = fileIn.ReadLine().Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries);
                List<Edge> Edges = new List<Edge>();
                foreach (string temp in tempArr[1].Split(new char[] { '_' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    Edges.Add(new Edge(int.Parse(temp)));
                }
                vertices.Add(int.Parse(tempArr[0]), Edges);
            }
            fileIn.Close();

            if (!isOriented)
            {
                foreach (var vertex in vertices)
                {
                    foreach (Edge edge in vertex.Value)
                    {
                        if (edge.PairId == 0 && edge.ConnectedVertex != vertex.Key)//если нет пары и при этом не петля
                        {
                            edge.PairId = CurrentPairId;
                            vertices[edge.ConnectedVertex].Find((Edge edge1) =>
                                edge1.ConnectedVertex == vertex.Key && edge1.PairId == 0).PairId = CurrentPairId;
                            CurrentPairId++;
                        }
                    }
                }
            }
        }

        public bool AddVertex(int id)
        {
            if (!vertices.ContainsKey(id))
            {
                vertices.Add(id, new List<Edge>());
                return true;
            }
            else return false;
        }
        public bool DeleteVertex(int id)
        {
            if (vertices.Remove(id))
            {
                foreach (var vertex in vertices)
                {
                    vertex.Value.RemoveAll((Edge edge) => edge.ConnectedVertex == id);
                }
                return true;
            }
            else return false;
        }

        public bool CreateEdge(int v1, int v2)
        {
            if (isOriented && isMultigraph)
            {
                if (vertices.ContainsKey(v1) && vertices.ContainsKey(v2))
                {
                    vertices[v1].Add(new Edge(v2));
                    return true;
                }
                else return false;
            }
            else if (isOriented && !isMultigraph)
            {
                if (vertices.ContainsKey(v1) && vertices.ContainsKey(v2)
                    && (vertices[v1].Find((Edge edge) => edge.ConnectedVertex == v2) == null))
                {
                    vertices[v1].Add(new Edge(v2));
                    return true;
                }
                else return false;
            }
            else if (!isOriented && isMultigraph)
            {
                if (vertices.ContainsKey(v1) && vertices.ContainsKey(v2))
                {
                    vertices[v1].Add(new Edge(v2, CurrentPairId));
                    if (v1 != v2)//если петля - не добавляем "встречное" ребро
                    {
                        vertices[v2].Add(new Edge(v1, CurrentPairId));
                        CurrentPairId++;
                    }
                    return true;
                }
                else return false;
            }
            else//!isOriented && !isMultigraph
            {
                if (vertices.ContainsKey(v1) && vertices.ContainsKey(v2)
                    && (vertices[v1].Find((Edge edge) => edge.ConnectedVertex == v2) == null))
                {
                    vertices[v1].Add(new Edge(v2, CurrentPairId));
                    if (v1 != v2)//если петля - не добавляем "встречное" ребро
                    {
                        vertices[v2].Add(new Edge(v1, CurrentPairId));
                        CurrentPairId++;
                    }
                    return true;
                }
                else return false;
            }
        }

        public bool DeleteEdge(int Id)//принимаем, что направление из v1 в connectedv
        {
            if (isOriented)
            {
                foreach (var vertex in vertices)
                {
                    if (vertex.Value.Remove(
                            vertex.Value.Find((Edge edge) => edge.Id == Id)))
                    {
                        return true;
                    }
                }
                return false;
            }
            else//!isOriented
            {
                foreach (var vertex in vertices)
                {
                    if (vertex.Value.Find((Edge edge) => edge.Id == Id) != null)//если найден нужный ID
                    {
                        if (vertex.Value.Find((Edge edge) => edge.Id == Id).ConnectedVertex == vertex.Key)//если петля
                        {
                            vertex.Value.Remove(
                                vertex.Value.Find((Edge edge) => edge.Id == Id));//удаляем ребро-петлю
                            return true;
                        }
                        else//если нужно удалить пару
                        {
                            int pair = vertex.Value.Find((Edge edge) => edge.Id == Id).PairId;
                            int connectedv = vertex.Value.Find((Edge edge) => edge.Id == Id).ConnectedVertex;
                            vertex.Value.Remove(vertex.Value.Find((Edge edge) => edge.Id == Id));//удаляем ребро с указанным ID
                            vertices[connectedv].Remove(vertices[connectedv].Find((Edge edge) => edge.PairId == pair));
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public bool AddEdgeWeight(int Id, int weight)
        {
            foreach (var vertex in vertices)
            {
                if (vertex.Value.Find((Edge edge) => edge.Id == Id) != null)//если найден нужный ID
                {
                    if (vertex.Value.Find((Edge edge) => edge.Id == Id).PairId != 0)//если есть пара
                    {
                        int pair = vertex.Value.Find((Edge edge) => edge.Id == Id).PairId;
                        int connectedv = vertex.Value.Find((Edge edge) => edge.Id == Id).ConnectedVertex;
                        vertex.Value.Find((Edge edge) => edge.Id == Id).Weight = weight;//назначаем вес ребру с указанным ID
                        vertices[connectedv].Find((Edge edge) => edge.PairId == pair).Weight = weight;
                        return true;
                    }
                    else//если петля (нет пары)
                    {
                        vertex.Value.Find((Edge edge) => edge.Id == Id).Weight = weight;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool ContainsLoops()
        {
            foreach (var vertex in vertices)
            {
                if (vertex.Value.Find((Edge edge) => edge.ConnectedVertex == vertex.Key) != null)
                {
                    return true;
                }
            }
            return false;
        }

        public static Graph CompleteGraph(Graph graph)//полный граф
        {
            if (!graph.isMultigraph && !graph.ContainsLoops())
            {
                Graph newGraph = new Graph(graph);
                foreach (var vertex in newGraph.vertices)
                {
                    foreach (var vertex2 in newGraph.vertices)
                    {
                        if (vertex.Key != vertex2.Key)
                        {
                            newGraph.CreateEdge(vertex.Key, vertex2.Key);
                        }
                    }
                }
                return newGraph;
            }
            else return null;
        }
        public static Graph ComplementOfGraph(Graph graph)//дополнение графа
        {
            if (!graph.isMultigraph && !graph.isOriented && !graph.ContainsLoops())
            {
                Graph newGraph = Graph.CompleteGraph(graph);
                foreach (var vertex in graph.vertices)
                {
                    foreach (Edge edge in vertex.Value)
                    {
                        newGraph.vertices[vertex.Key].RemoveAll((Edge tempEdge) => tempEdge.PairId == edge.PairId);
                    }
                }
                return newGraph;
            }
            else if (!graph.isMultigraph && graph.isOriented && !graph.ContainsLoops())
            {
                Graph newGraph = Graph.CompleteGraph(graph);
                foreach (var vertex in graph.vertices)
                {
                    foreach (Edge edge in vertex.Value)
                    {
                        newGraph.vertices[vertex.Key].RemoveAll((Edge tempEdge) => 
                            tempEdge.ConnectedVertex == edge.ConnectedVertex);
                    }
                }
                return newGraph;
            }
            else return null;
        }

        public static Graph GraphUnification(Graph graph1, Graph graph2)//объединение графов
        {
            foreach (var vertex in graph1.vertices)
            {
                if (graph2.vertices.ContainsKey(vertex.Key))
                {
                    return null;
                }
            }
            if ((!graph1.isMultigraph && !graph2.isMultigraph) && graph1.isOriented == graph2.isOriented)
            {
                Graph newGraph = new Graph(graph1);
                foreach (var vertex in graph2.vertices)
                {
                    newGraph.vertices.Add(vertex.Key, new List<Edge>());//копирование вершины и её рёбер
                    foreach (Edge edge in vertex.Value)
                    {
                        newGraph.vertices[vertex.Key].Add(new Edge(edge));
                    }
                }
                return newGraph;
            }
            else return null;
        }

        public static Graph GraphConnection(Graph graph1, Graph graph2)//соединение графов
        {
            Graph newGraph = Graph.GraphUnification(graph1, graph2);
            if (newGraph == null)
            {
                return null;
            }
            if (!newGraph.isOriented)
            {
                foreach (var vertex1 in graph1.vertices)
                {
                    foreach (var vertex2 in graph2.vertices)
                    {
                        newGraph.CreateEdge(vertex1.Key, vertex2.Key);
                    }
                }
            }
            else 
            {
                foreach (var vertex1 in graph1.vertices)
                {
                    foreach (var vertex2 in graph2.vertices)
                    {
                        newGraph.CreateEdge(vertex1.Key, vertex2.Key);
                        newGraph.CreateEdge(vertex2.Key, vertex1.Key);
                    }
                }
            }
            return newGraph;
        }

        public static void ContainsIsolatedVertices(Graph graph)
        {
            if (!graph.isOriented)
            {
                foreach (var vertex in graph.vertices)
                {
                    if (vertex.Value.Find((Edge edge) => edge.ConnectedVertex != vertex.Key) != null)
                    //если у вершины найдено ребро, не являющееся петлёй 
                    { }
                    else Console.WriteLine($"вершина: {vertex.Key}");
                }
            }
            else
            {
                foreach (var vertex in graph.vertices)
                    //для каждой вершины ищем в списках смежности других вершин соединяющее их ребро
                {
                    bool connected = false;
                    foreach (var vertex2 in graph.vertices)
                    {
                        if (vertex.Key != vertex2.Key //если не одна и та же вершина
                            && ((vertex2.Value.Find((Edge edge) => edge.ConnectedVertex == vertex.Key) != null) ||
                            (vertex.Value.Find((Edge edge) => edge.ConnectedVertex == vertex2.Key) != null)))
                        //если у вершины vertex2 найдено ребро, соединяющее его с vertex или ребро в обратном направлении у vertex
                        {
                            connected = true;
                            break;
                        }
                    }
                    if(!connected) Console.WriteLine($"вершина: {vertex.Key}");
                }
            }
        }

        public static void MaxDegree(Graph graph)
        {
            if (!graph.isOriented)
            {
                int max = 0;
                foreach (var vertex in graph.vertices)
                {
                    if (vertex.Value.Count > max)
                    {
                        max = vertex.Value.Count;
                    }
                }
                Console.WriteLine($"вершины с наибольшей степенью:");
                foreach (var vertex in graph.vertices)
                {
                    if (vertex.Value.Count == max)
                    {
                        Console.WriteLine($"{vertex.Key}");
                    }
                }
            }
            else 
            {
                Dictionary <int, int> degrees = new Dictionary<int, int>();
                foreach (var vertex in graph.vertices) 
                {
                    degrees.Add(vertex.Key, vertex.Value.Count);//количество всех исходящих рёбер
                    foreach (var vertex2 in graph.vertices)
                    {
                        if (vertex.Key != vertex2.Key)//пропускаем сравнение одной и той же вершины
                        {
                            degrees[vertex.Key] += vertex2.Value.FindAll((Edge edge) => edge.ConnectedVertex == vertex.Key).Count;
                        }
                    }
                }
                int max = 0;
                foreach (var vertex in degrees)
                {
                    if (vertex.Value > max)
                    {
                        max = vertex.Value;
                    }
                }
                Console.WriteLine($"вершины с наибольшей степенью:");
                foreach (var vertex in degrees)
                {
                    if (vertex.Value == max)
                    {
                        max = vertex.Value;
                        Console.WriteLine($"{vertex.Key}");
                    }
                }
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Graph graph = new Graph("E:\\учеба\\3 курс\\графы\\graph\\graph\\input_1.txt");
            GraphInterface Igraph = new GraphInterface(graph, "E:\\учеба\\3 курс\\графы\\graph\\graph\\output.txt");
            Graph graph1 = new Graph("E:\\учеба\\3 курс\\графы\\graph\\graph\\input_2.txt");

            Graph graph2 = new Graph("E:\\учеба\\3 курс\\графы\\graph\\graph\\input_3.txt");

            GraphInterface Igraph2 = new GraphInterface(Graph.GraphConnection(graph1, graph2),//GraphUnification GraphConnection
            "E:\\учеба\\3 курс\\графы\\graph\\graph\\output.txt");
            if (Igraph2.graph == null)
            {
                Console.WriteLine("Возникли ошибки при создании объединения графов");
            }
            else while (Igraph2.Operation(true)) { }
            //Graph.ContainsIsolatedVertices(graph);
            //Graph.MaxDegree(graph);
            //while (Igraph.Operation(true)) { }
        }
    }
}
