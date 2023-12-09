﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration; 
namespace TransportManagement.Models
{
    public class DAL
    {
        //The main purpose of the DAL is to play with the database. So declare the interaction tools at first
        private String Server {  get; set; } //Server of the database
        private string User {  get; set; }  //Username to connect to
        private string Port { get; set; }   //Port to connect to the databse
        private string Password { get; set; }   //Password to connect to the database
        private string DatabaseName { get; set; }   //Database name to connect to the database

        public DAL() 
        {
            
        }

        private void LoadConnectionString()
        {
            try
            {
                Server = ConfigurationManager.AppSettings["Server"];
                User = ConfigurationManager.AppSettings["User"];
                Port = ConfigurationManager.AppSettings["Port"];
                Password = ConfigurationManager.AppSettings["Password"];
                DatabaseName = ConfigurationManager.AppSettings["DatabaseName"];
                
                if(Server == null || User == null || Port == null || Password == null || DatabaseName == null) 
                {
                    throw new Exception("Could not retrieve the information about the database for establishing connection!!"); 
                }
            }

            catch (Exception ex) 
            {
                Logger.Log(ex.Message, LogLevel.Fatal);
                throw; 
            }   
        }

        //Populate the connection string to establish connection to the database. 
        public string ConnectionString()
        {
            string connectionString = $"server={Server};user={User}; port={Port};password={Password};database={DatabaseName}";
            return connectionString;
        }


    }
}
