using System;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using Npgsql;

namespace ServiceLibrary
{
    public class ImportService
    {
        private static readonly String CONN = "Host=localhost;Port=5432;Username=danielb;Password=pwd135;Database=mydb";
        private static readonly String SQL_INSERT = "INSERT INTO BALANCES (first_name, last_name, address, city, state, zip, phone, balance, createtime, filename) " +
                                                    "VALUES(@first, @last, @addr, @city, @state, @zip, @phone, @bal, @create, @filen)";

        private static readonly int FIRST = 0;
        private static readonly int LAST = 1;
        private static readonly int ADDR = 2;
        private static readonly int CITY = 3;
        private static readonly int STATE = 4;
        private static readonly int ZIP = 5;
        private static readonly int PHONE = 6;
        private static readonly int BAL = 7;

        private String importFile;
        private DateTime dateTime;

        public ImportService()
        {
            importFile = "No Assigned";
            dateTime = DateTime.Now;
        }

        public ImportService(String importFile, DateTime dateTime)
        {
            this.importFile = importFile;
            this.dateTime = dateTime;

        }

        public int ProcessImport()
        {
            Console.WriteLine("ImportService: \tfilename: {0} \tdatetime: {1}", importFile, dateTime);

            String bal;
            int iRead = 0;
            int iProc = 0;

            if (File.Exists(importFile))
            {
                var Lines = File.ReadAllLines(importFile);
                if (Lines.Length > 0)
                {
                    try
                    {
                        using (NpgsqlConnection conn = new NpgsqlConnection(CONN))
                        {
                            conn.Open();
                            NpgsqlCommand cmd = new NpgsqlCommand();
                            cmd.Connection = conn;
                            cmd.CommandText = SQL_INSERT;
                            cmd.CommandType = CommandType.Text;

                            foreach (var line in Lines)
                            {
                                iRead = iRead + 1;
                                if (iRead > 1)
                                {
                                    Console.WriteLine(line); //Debug
                                    var values = line.Split(',');

                                    cmd.Parameters.Add(new NpgsqlParameter("first", values[FIRST]));
                                    cmd.Parameters.Add(new NpgsqlParameter("last", values[LAST]));
                                    cmd.Parameters.Add(new NpgsqlParameter("addr", values[ADDR]));
                                    cmd.Parameters.Add(new NpgsqlParameter("city", values[CITY]));
                                    cmd.Parameters.Add(new NpgsqlParameter("state", values[STATE]));
                                    cmd.Parameters.Add(new NpgsqlParameter("zip", values[ZIP]));
                                    cmd.Parameters.Add(new NpgsqlParameter("phone", values[PHONE]));
                                    bal = Regex.Replace(values[BAL], "[^.0-9]", "");
                                    cmd.Parameters.Add(new NpgsqlParameter("bal", Decimal.Parse(bal)));
                                    cmd.Parameters.Add(new NpgsqlParameter("create", dateTime));
                                    cmd.Parameters.Add(new NpgsqlParameter("filen", importFile));
                                    cmd.Prepare();
                                    cmd.ExecuteNonQuery();
                                    cmd.Parameters.Clear();

                                    iProc = iProc + 1;
                                }
                            }

                            conn.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception: " + ex.ToString());
                    }

                }

                Console.WriteLine("Lines read: " + iRead + " Processed: " + iProc);

            }
            else
            {
                Console.WriteLine(importFile + " does not exist");

            }

            return iProc;
        }
    }
}
