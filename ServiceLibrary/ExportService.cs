using System;
using System.Data;
using System.IO;
using Npgsql;

namespace ServiceLibrary
{
    public class ExportService
    {
        private String importFile;
        private DateTime dateTime;
        private String exportFile;

        private static readonly String CONN = "Host=localhost;Port=5432;Username=danielb;Password=pwd135;Database=mydb";
        private static readonly String HEADER = "STATE,MIN_BALANCE,MAX_BALANCE,MEAN_BALANCE,TOTAL_BALANCE";
        private static readonly String SQL_SELECT = "select b.state, min(b.balance) min_balance, max(b.balance) max_balance, avg(b.balance) mean_balance, sum(b.balance) total_balance \n" +
                                                    "from balances b \n" +
                                                    "where b.createtime = @dt and b.filename = @fn \n" +
                                                    "group by b.state \n" +
                                                    "order by b.state";
        private static readonly int STATE = 0;
        private static readonly int MIN_BALANCE = 1;
        private static readonly int MAX_BALANCE = 2;
        private static readonly int MEAN_BALANCE = 3;
        private static readonly int TOTAL_BALANCE = 4;


        public ExportService()
        {
            importFile = "No Assigned";
            dateTime = DateTime.Now;
            exportFile = "No Assigned";
        }

        public ExportService(String importFile, DateTime dateTime, String exportFile)
        {
            this.importFile = importFile;
            this.dateTime = dateTime;
            this.exportFile = exportFile;
        }

        public int ProcessExport()
        {
            int iRead = 0;
            if (File.Exists(exportFile))
            {
                File.Delete(exportFile);
            }
            StreamWriter outfile = File.CreateText(exportFile);

            Console.WriteLine("ExportService: \timportFile: {0} \tdatetime: {1} \texportFile: {2}", importFile, dateTime, exportFile);  //Debug

            using (NpgsqlConnection conn = new NpgsqlConnection(CONN))
            {
                conn.Open();
                NpgsqlCommand cmd = new NpgsqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = SQL_SELECT;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new NpgsqlParameter("dt", dateTime));
                cmd.Parameters.Add(new NpgsqlParameter("fn", importFile));

                using NpgsqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    iRead = iRead + 1;
                    if (iRead == 1)
                    {
                        outfile.WriteLine(HEADER);
                    }

                    double meanBal = Math.Round(rdr.GetDouble(MEAN_BALANCE), 2, MidpointRounding.AwayFromZero);

                    outfile.WriteLine("{0},{1},{2},{3},{4}", rdr.GetString(STATE), rdr.GetDouble(MIN_BALANCE), rdr.GetDouble(MAX_BALANCE), meanBal, rdr.GetDouble(TOTAL_BALANCE));
                    Console.WriteLine("{0},{1},{2},{3},{4}", rdr.GetString(STATE), rdr.GetDouble(MIN_BALANCE), rdr.GetDouble(MAX_BALANCE), meanBal, rdr.GetDouble(TOTAL_BALANCE));  //Debug

                }

                outfile.Flush();
                outfile.Close();

                conn.Close();
            }

            return iRead;
        }

    }
}
