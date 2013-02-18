import clr;
clr.AddReference("System");
clr.AddReference("System.Data");
clr.AddReference("Feng.Windows");

from System import *
from System.Collections.Generic import *
from System.Text import *
from System.Data.SqlClient import *
from System.IO import *
from System.Net import *
import Feng;
	
class Program(object):
	def __init__(self):
		self._ftpCon = "ftp://nbzs:qazwsxedc@17haha8.oicp.net:8021";
		self._backupDbName = "jkhd2"
		self._strCon = "Data Source=192.168.0.10,8033;Initial Catalog=master;User ID=sa;Password=qazwsxedc" # Change SQLDBSERVER to the name of the SQL Server you are using
		self._LogFile = "SQLBackup.log" # The location on the local Drive of the log files.
		self._backupDir = "\\\\192.168.0.10\\Backup\\" # The local drive to save the backups to 
		self._backupFtpDir = "SqlBackup/"
		self._DaysToKeep = 31 # Number of days to keep the daily backups for
		self._DayOfWeekToKeep = DayOfWeek.Sunday # Specify which daily backup to keep

	def Main(self, args):
		self._fnLog = self.RotateLog(FileInfo(self._LogFile), self._DaysToKeep)
		self.WriteLog("Starting Weekly Backup.", self._fnLog)
		self.Backup()
		self.WriteLog("Daily BackupFinished.", self._fnLog)

	def Backup(self):
        try:
            dateTimeName = DateTime.Now.ToString("yyyyMMdd")
            # need to specify here which databases you do not want to back up.
            #SqlCommand comSQL = new SqlCommand("select name from sysdatabases where name not in('tempdb','model','Northwind','AdventureWorks','master') order by name ASC", new SqlConnection(strCon)); 
            comSQL = SqlCommand("select name from sysdatabases where name in('" + self._backupDbName + "') order by name ASC", SqlConnection(self._strCon))
            comSQL.Connection.Open()
            dr = comSQL.ExecuteReader()
            while dr.Read():
                self.WriteLog("Backing Up Database - " + dr["name"], self._fnLog)
                if DateTime.Now.DayOfWeek != self._DayOfWeekToKeep:
                    self.WriteLog("Deleting Backup from " + self._DaysToKeep.ToString() + " days ago", self._fnLog)
                    oldfn = FileInfo(self._backupDir + dr["name"] + "\\" + dr["name"] + "_full_" + DateTime.Now.Subtract(TimeSpan.FromDays(14)).ToString("yyyyMMdd") + ".Bak")
                    #FTPDeleteFile(new Uri("ftp://" + ftpServerURI + "/" + (string)dr["name"] + "/" + oldfn.Name), new NetworkCredential(ftpUserID, ftpPassword));
                else:
                    self.WriteLog("Keeping Weekly Backup.", self._fnLog)
                fn = FileInfo(self._backupDir + dr["name"] + "\\" + dr["name"] + "_full_" + dateTimeName + ".bak")		
                if File.Exists(fn.FullName):
                    self.WriteLog("Deleting Backup Because it Already Exists.", self._fnLog)
                    File.Delete(fn.FullName)
                Directory.CreateDirectory(fn.DirectoryName)
                comSQL2 = SqlCommand("BACKUP DATABASE @db TO DISK = @fn;", SqlConnection(self._strCon))
                comSQL2.CommandTimeout = 360
                comSQL2.Connection.Open()
                comSQL2.Parameters.AddWithValue("@db", dr["name"])
                comSQL2.Parameters.AddWithValue("@fn", fn.FullName)
                self.WriteLog("Starting Backup", self._fnLog)
                comSQL2.ExecuteNonQuery()
                self.WriteLog("Backup Succeeded.", self._fnLog)
                comSQL2.Connection.Close()

                zipBak = Feng.Utils.CompressionHelper.ZipFromFile(self._backupDir + dr["name"] + "\\" + dr["name"] + "_full_" + dateTimeName + ".bak", \
																	self._backupDir + dr["name"] + "\\" + dr["name"] + "_full_" + dateTimeName + ".zip");


                fn = FileInfo(self._backupDir + dr["name"] + "\\" + dr["name"] + "_full_" + dateTimeName + ".zip")		
                self.WriteLog("Uploading Backup to FTP server", self._fnLog)
                #self.FTPDeleteFile(Uri("ftp://" + self._ftpServerURI + self._backupFtpDir + dr["name"] + "/" + fn.Name), NetworkCredential(self._ftpUserID, self._ftpPassword))                                
                #if self.FTPUploadFile("ftp://" + self._ftpServerURI + self._backupFtpDir + dr["name"], "/" + fn.Name, fn, NetworkCredential(self._ftpUserID, self._ftpPassword)):
                Feng.Utils.FtpHelper.Instance.TraceWriter = Console.Out;
                Feng.Utils.FtpHelper.Instance.UploadFile(self._ftpCon, fn.FullName, self._backupFtpDir + dr["name"] + "\\" + fn.Name);
                self.WriteLog("Upload Succeeded", self._fnLog);
            comSQL.Connection.Close();
        except Exception, ex:
            self.WriteLog("Backup Failed: " + ex.Message, self._fnLog);

    def FTPDeleteFile(self, serverUri, Cred):
		retVal = True
		response = None
		try:
			request = WebRequest.Create(serverUri)
			request.Method = WebRequestMethods.Ftp.DeleteFile
			request.Credentials = Cred
			response = request.GetResponse()
			response.Close()
		except Exception, ex:
			if ex.Message != "The remote server returned an error: (550) File unavailable (e.g., file not found, no access).":
				Console.WriteLine("Error in FTPDeleteFile - " + ex.Message)
				if response != None:
					response.Close()
				retVal = False
		finally:
            return retVal
							 

	def RotateLog(self, LogFileName, Days):
		fNew = LogFileName.Directory.ToString() + DateTime.Now.ToString("\\\\yyyyMMdd_") + LogFileName.Name
		fOld = LogFileName.Directory.ToString() + DateTime.Now.Subtract(TimeSpan.FromDays(Days)).ToString("\\\\yyyyMMdd_") + LogFileName.Name
		fOldRecycler = "C:\\RECYCLER\\" + DateTime.Now.Subtract(TimeSpan.FromDays(Days)).ToString("yyyyMMdd_") + LogFileName.Name
		if File.Exists(fOld):
			self.WriteLog("Deleting LogFile - " + fOld + " because it is over " + Days.ToString() + " Days old", fNew)
			File.Move(fOld, fOldRecycler)
		return fNew


	def WriteLog(self, s, fn):
		print(s);
		File.AppendAllText(fn, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss:ffff") + " - " + s + Environment.NewLine)

if __name__ == "<module>" or __name__ == "__builtin__" or __name__ == "__main__":        
	p = Program();
	p.Main('');

