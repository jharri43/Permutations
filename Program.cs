using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permutations
{
    class Program
    {
        static int num_cols = 0;
        static int num_rows = 0;
        static int[,] reference_matrix;
        static int[,] permutation_matrix;
        static int[] colList;
        static int permutation_counter = 0;
        static string matrix_value;
        static string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        static string main_html_path;
        static StringBuilder html_main = new StringBuilder();
        static int middleRequires;
        static int borderRequires;
        static int requires;
        static string border_type;
        static bool matrix_from_file;
        static bool openHtml;

        static void Main(string[] args)
        {
            #region Inputs and Outputs
            //Output file
            main_html_path = mydocpath + @"\Permutations.html";

            //Inputs
            num_cols = 5; //Random Cols
            num_rows = 5; //Random Rows
            middleRequires = 6; //Middle cell requires
            borderRequires = 3; //Border cell requires
            border_type = "collapse"; //Border style
            //border_type = "separate";
            openHtml = false;


            //Check if loading a matrix by file. If not use random
            if (loadInputFile())
            {
                matrix_from_file = true;
                loadMatrix();
            }
            else
            {
                if (num_cols <= 0 || num_rows <= 0)
                {
                    Console.WriteLine("Enter Rows and Columns > 0 ");
                    return;
                }

                reference_matrix = new int[num_cols, num_rows];
                permutation_matrix = new int[num_cols, num_rows];

                Random r = new Random();

                //Make random starting matrix
                for (int i = 0; i < num_cols; i++)
                {
                    for (int j = 0; j < num_rows; j++)
                    {
                        if (r.NextDouble() > 0.5)
                        {
                            reference_matrix[i, j] = 1;
                        }
                        else
                        {
                            reference_matrix[i, j] = 0;
                        }
                    }
                }
            }
            #endregion

            colList = new int[num_rows];
            for (int i = 0; i < num_cols; i++)
            {
                colList[i] = i;
            }

            initHtmlFile();
            DoPermutations();
            finishHtmlFile();
            saveMatrix();
        }

        static public Boolean NextPermutation(int[] numList)
        {
            /*
             Knuths
             1. Find the largest index j such that a[j] < a[j + 1]. If no such index exists, the permutation is the last permutation.
             2. Find the largest index l such that a[j] < a[l]. Since j + 1 is such an index, l is well defined and satisfies j < l.
             3. Swap a[j] with a[l].
             4. Reverse the sequence from a[j + 1] up to and including the final element a[n].

             */
            var largestIndex = -1;
            for (var i = numList.Length - 2; i >= 0; i--)
            {
                if (numList[i] < numList[i + 1])
                {
                    largestIndex = i;
                    break;
                }
            }

            if (largestIndex < 0)
            {
                return false;
            }

            var largestIndex2 = -1;
            for (var i = numList.Length - 1; i >= 0; i--)
            {
                if (numList[largestIndex] < numList[i])
                {
                    largestIndex2 = i;
                    break;
                }
            }

            var tmp = numList[largestIndex];
            numList[largestIndex] = numList[largestIndex2];
            numList[largestIndex2] = tmp;

            for (int i = largestIndex + 1, j = numList.Length - 1; i < j; i++, j--)
            {
                tmp = numList[i];
                numList[i] = numList[j];
                numList[j] = tmp;
            }
            //permutation = numList;
            return true;
        }

        static public int[] getColumn(int index)
        {
            int[] col = new int[num_rows];

            for (int i = 0; i < num_rows; i++)
            {
                col[i] = reference_matrix[index, i];
            }

            return col;
        }

        static public void DoPermutations()
        {
            permutation_counter = 1;

            //Do first one
            permutation_matrix = (int[,])reference_matrix.Clone();
            AppendHTMLTable(colList);

            //Get a new permutation of columns (1 2 4 3)
            while (NextPermutation(colList))
            {
                //Get each column of the new permutation and build a new matrix               
                for (int i = 0; i < colList.Length; i++)
                {
                    //Get one whole column of the matrix ( first column =  1 1 0 1)
                    int[] col_line = getColumn(colList[i]);

                    //Insert column into new matrix at a new position
                    for (int j = 0; j < col_line.Length; j++)
                    {
                        permutation_matrix[i, j] = col_line[j];
                    }
                }

                //Append HTML table of new matrix
                AppendHTMLTable(colList);

                permutation_counter++;
            }
        }

        static public void AppendHTMLTable(int[] colHeaders)
        {
            //StringBuilder html_main = new StringBuilder();

            html_main.AppendLine("<div class=\"tbl\">");

            html_main.AppendLine("<table>");

            //Build headers
            html_main.AppendLine("<tr>");
            for (int i = 0; i < colHeaders.Length; i++)
            {
                html_main.AppendLine("<th>" + (colHeaders[i] + 1).ToString() + "</th>");
            }
            html_main.AppendLine("</tr>");

            //Build each body row of the table
            for (int i = 0; i < num_rows; i++)
            {
                html_main.AppendLine("<tr>");
                for (int j = 0; j < num_cols; j++)
                {
                    matrix_value = permutation_matrix[j, i].ToString();
                    //Check if this 0 might should turn black to make a square
                    if (matrix_value == "0" && shouldBlack(j, i))
                    {
                        //permutation_matrix[j, i] = 1;//This would make this really count as a neighbor for future cells
                        matrix_value = "2";//Make it a different color to denote a change
                    }
                    html_main.AppendLine("<td class=\"c" + matrix_value + "\">" + matrix_value + "</td>");
                }
                html_main.AppendLine("</tr>");
            }

            html_main.AppendLine("</table><br/>");

            html_main.AppendLine("</div>");

            //Append to file every 1000 perms
            //if (permutation_counter % 1 == 0)
            //{
            using (StreamWriter sw = File.AppendText(main_html_path))
            {
                sw.WriteLine(html_main);
                html_main = new StringBuilder();
            }
            //}

        }

        static public int minus(int n)
        {
            return --n;
        }
        static public int plus(int n)
        {
            return ++n;
        }

        //Determine how many neighbors are required for me
        static public int getRequiredNeighbors(int col, int row)
        {
            //If you are on the border, you require less
            if (col == num_cols - 1 || col == 0 || row == 0 || row == num_rows - 1)
            {
                return borderRequires;
            }

            return middleRequires;
        }

        //If I have 3 neighbors that are black, maybe I should be changed to make a square 
        static public bool shouldBlack(int col, int row)
        {
            int num_black_neighbors = 0;
            bool shouldBlack = false;

            //See how many neighbors this index requires to fulfill requirement
            requires = getRequiredNeighbors(col, row);


            //Top Left       
            if (row > 0 && col > 0)
            {
                if (foundBlack(minus(col), minus(row)))
                {
                    if (checkDone(num_black_neighbors, out num_black_neighbors)) return true;
                }
            }
            //Top
            if (row > 0)
            {
                if (foundBlack(col, minus(row)))
                {
                    if (checkDone(num_black_neighbors, out num_black_neighbors)) return true;
                }
            }
            //Top Right       
            if (row > 0 && col < num_cols - 1)
            {
                if (foundBlack(plus(col), minus(row)))
                {
                    if (checkDone(num_black_neighbors, out num_black_neighbors)) return true;
                }
            }
            //Right
            if (col < num_cols - 1)
            {
                if (foundBlack(plus(col), row))
                {
                    if (checkDone(num_black_neighbors, out num_black_neighbors)) return true;
                }
            }
            //Bottom Right
            if (row < num_rows - 1 && col < num_cols - 1)
            {
                if (foundBlack(plus(col), plus(row)))
                {
                    if (checkDone(num_black_neighbors, out num_black_neighbors)) return true;
                }
            }
            //Bottom
            if (row < num_rows - 1)
            {
                if (foundBlack(col, plus(row)))
                {
                    if (checkDone(num_black_neighbors, out num_black_neighbors)) return true;
                }
            }
            //Botton Left
            if (row > 0 && col > 0 && plus(row) < num_rows)
            {
                if (foundBlack(minus(col), plus(row)))
                {
                    if (checkDone(num_black_neighbors, out num_black_neighbors)) return true;
                }
            }
            //Left
            if (col > 0)
            {
                if (foundBlack(minus(col), row))
                {
                    if (checkDone(num_black_neighbors, out num_black_neighbors)) return true;
                }
            }




            return shouldBlack;
        }


        static public bool foundBlack(int col, int row)
        {
            bool is_black = false;
            if (permutation_matrix[col, row] == 1)
            {
                is_black = true;
            }
            return is_black;
        }

        static public bool checkDone(int n, out int new_num)
        {
            bool done = false;
            new_num = n;
            new_num++;
            if (new_num == requires)//Number of neighbors required
            {
                return true;
            }
            return done;
        }



        static public void initHtmlFile()
        {
            if (File.Exists(main_html_path))
            {
                File.Delete(main_html_path);
                var myFile = File.Create(main_html_path);
                myFile.Close();


                string s = "<!DOCTYPE html><html><head><meta http-equiv=\"content-type\" content=\"text/html; charset=utf-8\" />";
                s += "<title>Permutations</title><style type=\"text/css\">.c1{background-color: black}.c0{color: white}.c2{background-color: green;color: green}.tbl{float:left;padding:5px}table{table-layout:fixed;width:110px}table,td,th{border: 1px solid black;border-collapse: " + border_type + "}</style></head>";
                using (StreamWriter sw = File.AppendText(main_html_path))
                {
                    sw.WriteLine(s);
                }
            }
        }

        static public void finishHtmlFile()
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine("<p> Total Permutations " + permutation_counter + "</p>");
            s.AppendLine("</html>");

            using (StreamWriter sw = File.AppendText(main_html_path))
            {
                sw.WriteLine(s);
            }

            //Open file
            if (openHtml)
            {
              System.Diagnostics.Process.Start(main_html_path);  
            }
            
        }

        //Save the original input to file
        static public void saveMatrix()
        {
            //Don't resave. This has already been moved back
            if (matrix_from_file)
            {
                return;
            }
            string matrix_row = "";
            string outputrun = mydocpath + @"\PermRuns\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt";
            var myFile = File.Create(outputrun);
            myFile.Close();

            using (StreamWriter sw = File.AppendText(outputrun))
            {
                for (int i = 0; i < num_rows; i++)
                {
                    matrix_row = "";
                    for (int j = 0; j < num_cols; j++)
                    {
                        matrix_row = matrix_row + reference_matrix[j, i].ToString();
                    }
                    sw.WriteLine(matrix_row);
                }
            }
        }


        //Load matrix from file if one is present
        static public void loadMatrix()
        {
            string[] dirs = Directory.GetFiles(mydocpath + @"\PermInput\");
            if (dirs.Length == 0)
            {
                return;
            }
            string line = "";
            int number = 0;
            int line_counter = 0;

            using (StreamReader reader = new StreamReader(dirs[0]))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    for (int i = 0; i < num_cols; i++)
                    {
                        number = int.Parse(line.Substring(i, 1));
                        reference_matrix[i, line_counter] = number;
                    }

                    line_counter++;
                }
            }

            //Delete the file if it exists in PermRuns
            if (File.Exists(mydocpath + @"\PermRuns\" + Path.GetFileName(dirs[0])))
            {
                File.Delete(mydocpath + @"\PermRuns\" + Path.GetFileName(dirs[0]));
            }

            //Move the file
            File.Move(dirs[0], mydocpath + @"\PermRuns\" + Path.GetFileName(dirs[0]));
            return;
        }


        //See if there is an input file
        static public bool loadInputFile()
        {
            string[] dirs;
            try
            {
                dirs = Directory.GetFiles(mydocpath + @"\PermInput\");
            }
            catch (Exception)
            {
                return false;
            }

            if (dirs.Length == 0)
            {
                return false;
            }
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(dirs[0]);
            int row_count = 0;
            while ((line = file.ReadLine()) != null)
            {
                num_cols = line.Length;
                row_count++;
            }
            num_rows = row_count;
            reference_matrix = new int[num_cols, num_rows];
            permutation_matrix = new int[num_cols, num_rows];
            file.Close();

            return true;
        }

    }
}
