using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Data.SqlClient;
using System.Threading;
using System.Data;

namespace BLayer
{
    /// <summary>
    /// Summary description for BLayer
    /// </summary>
    [WebService(Namespace = "https://adc558.azurewebsites.net/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    //[System.Web.Script.Services.ScriptService]
    public class BLayer : System.Web.Services.WebService
    {
        public delegate bool ValidateLoginAsyncStub(string student_id, string password);
        public delegate bool SignUpAsyncStub(string first_name, string last_name, string student_id, string password);
        public delegate bool completeProfileAsyncStub(string phone_no, string address, string city, string country, string state, string emergency_contact, string relationship, string email_id, string student_id);

        public class MyState
        {
            public object previousState;
            public ValidateLoginAsyncStub asyncStub;
            public SignUpAsyncStub asyncStub_SignUp;
            public completeProfileAsyncStub asyncStub_completeProf;
        }

        [WebMethod]
        public IAsyncResult BeginValidateLogin(string student_id, string password,
            AsyncCallback cb, object s)
        {
            ValidateLoginAsyncStub stub
                = new ValidateLoginAsyncStub(ValidateLogin);
            MyState ms = new MyState();
            ms.previousState = s;
            ms.asyncStub = stub;
            return stub.BeginInvoke(student_id, password, cb, ms);
        }

        [WebMethod]
        public bool EndValidateLogin(IAsyncResult call)
        {
            MyState ms = (MyState)call.AsyncState;
            return ms.asyncStub.EndInvoke(call);
        }
        public bool ValidateLogin(string student_id, string password)
        {
            student_id = student_id.Trim();
            string connectionString =
       "Server=tcp:adcproj.database.windows.net,1433;" +
       "Initial Catalog=adc;Persist Security Info=False;" +
       "User ID=adcproj;Password=Test1234;MultipleActiveResultSets=False;" +
       "Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";


            string queryString = string.Format("select student_id,password from dbo.student Where student_id={0} AND password ='{1}'", student_id, password);

            using (SqlConnection connection =
         new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                SqlCommand command = new SqlCommand(queryString, connection);

                // await connection.OpenAsync();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    Console.WriteLine("true");
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }
        [WebMethod]
        public IAsyncResult BeginSignUp(string student_id, string first_name, string last_name,  string password,
            AsyncCallback cb, object s)
        {
            SignUpAsyncStub stub
                = new SignUpAsyncStub(SignUp);
            MyState ms = new MyState();
            ms.previousState = s;
            ms.asyncStub_SignUp = stub;
            return stub.BeginInvoke(student_id, first_name, last_name,  password, cb, ms);
        }

        [System.Web.Services.WebMethod]
        public bool EndSignUp(IAsyncResult call)
        {
            MyState ms = (MyState)call.AsyncState;
            return ms.asyncStub_SignUp.EndInvoke(call);
        }
        public bool SignUp(string student_id, string first_name, string last_name,  string password)
        {
            string connectionString =
       "Server=tcp:adcproj.database.windows.net,1433;" +
       "Initial Catalog=adc;Persist Security Info=False;" +
       "User ID=adcproj;Password=Test1234;MultipleActiveResultSets=False;" +
       "Encrypt=True;TrustServerCertificate=False;Connection Timeout=60;";

            // Provide the query string with a parameter placeholder.
            //string queryString = "INSERT INTO dbo.student (first_name,last_name, student_id,password) VALUES (@val1, @val2, @val3,@val4)";

            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionString))

                {
                    using (SqlCommand cmd = new SqlCommand("studentSignUp", cnn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;                       
                        cmd.Parameters.Add(new SqlParameter("@student_id", student_id));
                        cmd.Parameters.Add(new SqlParameter("@first_name", first_name));
                        cmd.Parameters.Add(new SqlParameter("@last_name", last_name));
                        cmd.Parameters.Add(new SqlParameter("@email_id", DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@phone_no", DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@password", password));
                        cmd.Parameters.Add(new SqlParameter("@address", DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@city", DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@state", DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@country", DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@emergency_contact", DBNull.Value));
                        cmd.Parameters.Add(new SqlParameter("@relationship", DBNull.Value));

                        cnn.Open();
                        SqlParameter returnParameter = cmd.Parameters.Add("Return Value", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        cmd.ExecuteNonQuery();
                        int id = (int)returnParameter.Value;
                        
                        cnn.Close();

                        if (id >= 0)
                            return true;
                        else
                            return false;
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }
        [WebMethod]
        public IAsyncResult BegincompleteProfile(string phone_no, string address, string city, string country, string state, string emergency_contact, string relationship, string email_id, string student_id,
            AsyncCallback cb, object s)
        {
            completeProfileAsyncStub stub
                = new completeProfileAsyncStub(completeProfile);
            MyState ms = new MyState();
            ms.previousState = s;
            ms.asyncStub_completeProf = stub;
            return stub.BeginInvoke(phone_no, address, city, country, state, emergency_contact, relationship, email_id, student_id, cb, ms);
        }

        [System.Web.Services.WebMethod]
        public bool EndcompleteProfile(IAsyncResult call)
        {
            MyState ms = (MyState)call.AsyncState;
            return ms.asyncStub_completeProf.EndInvoke(call);
        }
        public bool completeProfile(string phone_no, string address, string city, string country, string state, string emergency_contact, string relationship, string email_id, string student_id)
        {
            string connectionString = "Server=tcp:adcproj.database.windows.net,1433;" +
"Initial Catalog=adc;Persist Security Info=False;" +
"User ID=adcproj;Password=Test1234;MultipleActiveResultSets=False;" +
"Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            // Provide the query string with a parameter placeholder.

            string queryString = string.Format("UPDATE dbo.student SET phone_no ={0}, address = '{1}', city = '{2}', state = '{3}', country = '{4}', emergency_contact ={5}, email_id ='{6}', relationship ='{7}' WHERE student_id = {8}", phone_no, address, city, state, country, emergency_contact, email_id, relationship, student_id);


            try
            {
                using (SqlConnection cnn = new SqlConnection(connectionString))

                {
                    using (SqlCommand cmd = new SqlCommand(queryString, cnn))
                    {
                        cnn.Open();

                        int rows = cmd.ExecuteNonQuery();
                        cnn.Close();
                        if (rows >= 1)
                            return true;
                        else
                            return false;

                    }

                }
            }
            catch (SystemException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }

    }
}
