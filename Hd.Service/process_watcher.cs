using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.IO;
using System.Security.Permissions;
using Feng.Data;
using Feng;

namespace Hd.Service
{
    public class process_watcher
    {
        private const string localAttachmentPath = "附件";    // 程序根目录下，附件的保存路径

        #region OpenFileToAttachment 打开附件

        public static void OpenFileToAttachment(int Id)
        {
            if (!Directory.Exists(localAttachmentPath))
            {
                Directory.CreateDirectory(localAttachmentPath);
            }

            using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<AttachmentInfo>())
            {
                AttachmentInfo att = rep.Get<AttachmentInfo>(Convert.ToInt64(Id));
                OpenFileToAttachment(att);
            }
        }

        public static void OpenFileToAttachment(string entityId)
        {
            if (!Directory.Exists(localAttachmentPath))
            {
                Directory.CreateDirectory(localAttachmentPath);
            }

            using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<AttachmentInfo>())
            {
                IList<AttachmentInfo> list = (rep as Feng.NH.INHibernateRepository).List<AttachmentInfo>(NHibernate.Criterion.DetachedCriteria.For<AttachmentInfo>()
                    .Add(NHibernate.Criterion.Expression.Eq("EntityId", entityId)));
                foreach (AttachmentInfo att in list)
                {
                    OpenFileToAttachment(att);
                }
            }
        }

        public static void OpenFileToAttachment(AttachmentInfo att)
        {
            if (att == null)
            {
                return;
            }
            if (!Directory.Exists(localAttachmentPath + "\\" + att.EntityId))
            {
                Directory.CreateDirectory(localAttachmentPath + "\\" + att.EntityId);
            }
            FileStream fs = new FileStream(localAttachmentPath + "\\" + att.EntityId + "\\" + att.FileName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(att.Data, 0, att.Data.Length);
            bw.Close();
            fs.Close();
            System.Diagnostics.Process.Start(localAttachmentPath + "\\" + att.EntityId + "\\" + att.FileName);
        }

        #endregion
    }

    /// <summary>
    /// 文件夹循环监视器
    /// </summary>
    public class ForWatcher
    {
        public ForWatcher(string path, string entityName, string propertyName)
        {
            m_Path = path;
            m_EntityName = entityName;
            m_PropertyName = propertyName;
        }

        private string m_Path = null;         // 监视路径
        private string m_EntityName = null;   // 实体名称，AttachmentInfo.EntityName
        private string m_PropertyName = null; // 属性名称，无用
        private Dictionary<string, IList<string>> fileList = new Dictionary<string, IList<string>>(); // 监视目录下所有文件列表，Dictionary<entityId, 文件路径列表>

        /// <summary>
        /// 监视路径
        /// </summary>
        public string MonitorPath
        {
            get { return m_Path; }
            set { m_Path = value; }
        }
        /// <summary>
        /// 实体名称，AttachmentInfo.EntityName
        /// </summary>
        public string EntityName
        {
            get { return m_EntityName; }
            set { m_EntityName = value; }
        }
        /// <summary>
        /// 属性名称，无用
        /// </summary>
        public string PropertyName
        {
            get { return m_PropertyName; }
            set { m_PropertyName = value; }
        }

        /// <summary>
        /// 开始循环监视一遍
        /// </summary>
        public void Run()
        {
            fileList.Clear();
            // 获取子文件夹中的文件列表
            string[] directories = Directory.GetDirectories(m_Path, "*", SearchOption.TopDirectoryOnly);
            foreach (string directory in directories)
            {
                fileList.Add(directory.Substring(directory.LastIndexOf("\\") + 1), Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories));
            }
            // 获取第一层文件列表
            string[] files = Directory.GetFiles(m_Path, "*.*", SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                fileList.Add(Path.GetFileNameWithoutExtension(file), new string[] { file });
            }

            foreach (KeyValuePair<string, IList<string>> fileListItem in fileList)
            {
                using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<AttachmentInfo>())
                {
                    IList<AttachmentInfo> attList = (rep as Feng.NH.INHibernateRepository)
                        .List<AttachmentInfo>(NHibernate.Criterion.DetachedCriteria.For<AttachmentInfo>()
                        .Add(NHibernate.Criterion.Expression.Eq("EntityId", fileListItem.Key)));

                    bool isSave = false;

                    // 数据库与文件夹内文件数量不同，全部Delete重新Save
                    if (attList != null && fileListItem.Value != null && attList.Count != fileListItem.Value.Count)
                    {
                        isSave = true;
                    }
                    #region 以文件为单位。一个文件等同于含有一个文件的文件夹

                    else if (fileListItem.Value != null && fileListItem.Value.Count == 1)
                    {
                        // 单个文件如不相同，Delete后重新Save
                        if (attList != null && attList.Count == 1)
                        {
                            if (File.Exists(fileListItem.Value[0]))
                            {
                                FileInfo fileInfo = new FileInfo(fileListItem.Value[0]);
                                if (fileInfo.Name != attList[0].FileName
                                    || fileInfo.CreationTime.ToString() != attList[0].Created.ToString()
                                    || fileInfo.LastWriteTime.ToString() != attList[0].Updated.ToString())
                                {
                                    isSave = true;
                                }
                            }
                        }
                    }

                    #endregion

                    #region 以文件夹为单位。含有多个文件的文件夹

                    else if (fileListItem.Value != null && fileListItem.Value.Count > 1)
                    {
                        // 文件数量相同，遍历所有文件，如有一个不同，全部Delete重新Save
                        if (attList != null && fileListItem.Value.Count == attList.Count)
                        {
                            foreach (string file in fileListItem.Value)
                            {
                                if (File.Exists(file))
                                {
                                    bool isSame = false;
                                    FileInfo fileInfo = new FileInfo(file);
                                    foreach (AttachmentInfo att in attList)
                                    {
                                        if (fileInfo.Name == att.FileName
                                            && fileInfo.CreationTime.ToString() == att.Created.ToString()
                                            && fileInfo.LastWriteTime.ToString() == att.Updated.ToString())
                                        {
                                            isSame = true;
                                            continue;
                                        }
                                    }
                                    if (!isSame)
                                    {
                                        isSave = true;
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    if (isSave)
                    {
                        rep.BeginTransaction();
                        foreach (AttachmentInfo att in attList)
                        {
                            rep.Delete(att);
                        }
                        rep.CommitTransaction();
                        SaveFiles(rep, fileListItem);
                    }
                }
            }
            fileList.Clear();
        }

        private byte[] ReadFile(string fullName)
        {
            if (!File.Exists(fullName)) return null;
            FileInfo fileInfo = new FileInfo(fullName);
            FileStream fs = fileInfo.OpenRead();
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            return bytes;
        }

        private void SaveFiles(IRepository rep, KeyValuePair<string, IList<string>> fileList)
        {
            foreach (string file in fileList.Value)
            {
                try
                {
                    rep.BeginTransaction();
                    if (File.Exists(file))
                    {
                        byte[] bytes = ReadFile(file);
                        if (bytes == null || bytes.Length == 0)
                        {
                            Console.WriteLine("ForWatcher Error:<" + fileList.Key + ">" + Environment.NewLine
                                + "Nor found the file or fileStream length is 0 \"" + file + "\"");
                            continue;
                        }
                        FileInfo fileInfo = new FileInfo(file);
                        AttachmentInfo newAtt = new AttachmentInfo();
                        newAtt.EntityName = m_EntityName;
                        newAtt.EntityId = fileList.Key;
                        newAtt.FileName = fileInfo.Name;
                        newAtt.Data = bytes;
                        newAtt.Description = "附件";
                        newAtt.Created = fileInfo.CreationTime;
                        newAtt.CreatedBy = "服务";
                        newAtt.Updated = fileInfo.LastWriteTime;
                        rep.Save(newAtt);
                    }
                    rep.CommitTransaction();
                }
                catch (Exception ex)
                {
                    rep.RollbackTransaction();
                    Console.WriteLine("ForWatcher Error:<" + fileList.Key + ">" + Environment.NewLine + ex.Message);
                    System.Windows.Forms.MessageBox.Show("<" + fileList.Key + ">" + Environment.NewLine + ex.Message, "ForWatcher Error");
                    continue;
                }
            }
        }
    }

    /// <summary>
    /// 文件夹监视器
    /// </summary>
    public class Watcher
    {
        public Watcher(string path, string entityName, string propertyName)
        {
            this.path = path;
            this.entityName = entityName;
            this.propertyName = propertyName;

            m_timer = new WatcherTimer(OnChanged, OnRenamed, 60000);
        }

        private string path = null; // 监视路径
        private string entityName = null;   // 实体名称，AttachmentInfo.EntityName
        private string propertyName = null; // 属性名称，无用
        private string directoryName = null;    // 文件夹名，AttachmentInfo.EntityId
        private WatcherTimer m_timer = null;    // 延迟器

        /// <summary>
        /// 开始监视文件夹
        /// </summary>
        /// <param name="path">监视路径</param>
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public void Run()
        {
            FileSystemWatcher watcher = new FileSystemWatcher(this.path);
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size |
                NotifyFilters.FileName | NotifyFilters.DirectoryName;

            watcher.Changed += new FileSystemEventHandler(m_timer.OnFileChanged);
            watcher.Created += new FileSystemEventHandler(m_timer.OnFileChanged);
            //watcher.Deleted += new FileSystemEventHandler(m_timer.OnFileChanged);
            watcher.Renamed += new RenamedEventHandler(m_timer.OnFileChanged);

            // 开始监视
            watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// 更改、创建或删除文件触发该事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            if (Directory.Exists(e.FullPath))
            {
                using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<AttachmentInfo>())
                {
                    try
                    {
                        directoryName = e.Name;
                        rep.BeginTransaction();
                        Deleted(rep, e);
                        foreach (string fileName in Directory.GetFiles(e.FullPath, "*.*", SearchOption.AllDirectories))
                        {
                            Created(rep, new FileSystemEventArgs(WatcherChangeTypes.Created, fileName.Substring(0, fileName.LastIndexOf('\\')), fileName.Substring(fileName.LastIndexOf('\\') + 1)));
                        }
                        rep.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        rep.RollbackTransaction();
                        Console.WriteLine("OnChanged Members of Watcher Error:" + ex.Message);
                        System.Windows.Forms.MessageBox.Show(ex.Message, "OnChanged Members of Watcher Error");
                    }
                }
            }
            else if (File.Exists(e.FullPath))
            {
                using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<AttachmentInfo>())
                {
                    try
                    {
                        directoryName = e.Name.Substring(0, e.Name.LastIndexOf('.'));
                        rep.BeginTransaction();
                        Deleted(rep, e);
                        Created(rep, e);
                        rep.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        rep.RollbackTransaction();
                        Console.WriteLine("OnChanged Members of Watcher Error:" + ex.Message);
                        System.Windows.Forms.MessageBox.Show(ex.Message, "OnChanged Members of Watcher Error");
                    }
                }
            }
            else if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<AttachmentInfo>())
                {
                    try
                    {
                        rep.BeginTransaction();
                        Deleted(rep, e);
                        rep.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        rep.RollbackTransaction();
                        Console.WriteLine("OnChanged Members of Watcher Error:" + ex.Message);
                        System.Windows.Forms.MessageBox.Show(ex.Message, "OnChanged Members of Watcher Error");
                    }
                }
            }
        }

        /// <summary>
        /// 重名命文件触发该事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnRenamed(object source, RenamedEventArgs e)
        {
            if (Directory.Exists(e.FullPath) || File.Exists(e.FullPath))
            {
                using (IRepository rep = ServiceProvider.GetService<IRepositoryFactory>().GenerateRepository<AttachmentInfo>())
                {
                    try
                    {
                        rep.BeginTransaction();
                        Renamed(rep, e);
                        rep.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        rep.RollbackTransaction();
                        Console.WriteLine("OnRenamed Members of Watcher Error:" + ex.Message);
                        System.Windows.Forms.MessageBox.Show(ex.Message, "OnRenamed Members of Watcher Error");
                    }
                }
            }
        }

        private void Created(IRepository rep, FileSystemEventArgs e)
        {
            try
            {
                FileInfo fi = new FileInfo(e.FullPath);
                FileStream fs = fi.OpenRead();
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                fs.Close();

                AttachmentInfo att = new AttachmentInfo();
                att.FileName = e.Name;
                att.EntityName = this.entityName;
                att.Data = bytes;
                att.Description = "扫描件";
                att.CreatedBy = "服务";
                att.EntityId = directoryName;
                att.Created = DateTime.Now;

                //Type type = Type.GetType(this.entityName, true, true);
                //PropertyInfo pro = type.GetProperty("Id");
                //string aa = fileName.Substring(0, fileName.LastIndexOf('.'));
                //IList list = (rep as Feng.NH.INHibernateRepository).Session.CreateCriteria(type)
                //    .Add(NHibernate.Criterion.Expression.Eq(propertyName, fileName.Substring(0, fileName.LastIndexOf('.')))).List();
                //IList<Hd.Model.Ck.出口票> list1 = (rep as Feng.NH.INHibernateRepository).Session.CreateCriteria<Hd.Model.Ck.出口票>()
                //   .Add(NHibernate.Criterion.Expression.Eq("货代自编号", "SB10114")).List<Hd.Model.Ck.出口票>();
                //IList list2 = (rep as Feng.NH.INHibernateRepository).Session.CreateCriteria(type).List();

                //if (list == null || list.Count == 0)
                //{
                //    att.EntityId = Guid.Empty.ToString();
                //}
                //else
                //{
                //    att.EntityId = pro.GetValue(list[0], null).ToString();
                //}

                rep.Save(att);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Greated Members of Watcher Error:" + ex.Message);
            }
        }

        private void Changed(IRepository rep, FileSystemEventArgs e)
        {
            try
            {
                IList<AttachmentInfo> list = (rep as Feng.NH.INHibernateRepository).List<AttachmentInfo>(NHibernate.Criterion.DetachedCriteria.For<AttachmentInfo>()
                    .Add(NHibernate.Criterion.Expression.Eq("FileName", e.Name)));

                if (list == null || list.Count == 0)
                {
                    Created(rep, e);
                }

                FileInfo fi = new FileInfo(e.FullPath);
                FileStream fs = fi.OpenRead();
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                fs.Close();

                list[0].Data = bytes;
                rep.Update(list[0]);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Changed Members of Watcher Error:" + ex.Message);
            }
        }

        private void Deleted(IRepository rep, FileSystemEventArgs e)
        {
            try
            {
                (rep as Feng.NH.INHibernateRepository).Session.Delete("from AttachmentInfo where EntityId = '" + directoryName + "'");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Deleted Members of Watcher Error:" + ex.Message);
            }
        }

        private void Renamed(IRepository rep, RenamedEventArgs e)
        {
            try
            {
                (rep as Feng.NH.INHibernateRepository).Session.CreateQuery("update AttachmentInfo set EntityId = :newId where EntityId = :oldId")
                    .SetString("newId", e.Name).SetString("oldId", e.OldName).ExecuteUpdate();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Renamed Members of Watcher Error:" + ex.Message);
            }
        }
    }

    /// <summary>
    /// Watcher的延时器
    /// </summary>
    public class WatcherTimer
    {
        private int TimeoutMillis = 2000;

        System.IO.FileSystemWatcher fsw = new System.IO.FileSystemWatcher();
        System.Threading.Timer m_timer = null;
        List<FileSystemEventArgs> files = new List<FileSystemEventArgs>();
        FileSystemEventHandler fswHandler = null;
        RenamedEventHandler reHandler = null;

        public WatcherTimer(FileSystemEventHandler changeHandler, RenamedEventHandler renamedHandler)
        {
            m_timer = new System.Threading.Timer(new TimerCallback(OnTimer), null, Timeout.Infinite, Timeout.Infinite);
            fswHandler = changeHandler;
            reHandler = renamedHandler;
        }

        public WatcherTimer(FileSystemEventHandler changeHandler, RenamedEventHandler renamedHandler, int timerInterval)
        {
            m_timer = new System.Threading.Timer(new TimerCallback(OnTimer), null, Timeout.Infinite, Timeout.Infinite);
            TimeoutMillis = timerInterval;
            fswHandler = changeHandler;
            reHandler = renamedHandler;
        }

        public void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            Mutex mutex = new Mutex(false, "FSW");
            mutex.WaitOne();
            //if (!files.Contains(e))
            //{
            //    files.Add(e);
            //}
            bool isSame = false;
            foreach (FileSystemEventArgs fsEvent in files)
            {
                isSame = fsEvent.Name == e.Name ? true : false;
            }
            if (!isSame)
            {
                files.Add(e);
            }
            mutex.ReleaseMutex();

            m_timer.Change(TimeoutMillis, Timeout.Infinite);
        }

        private void OnTimer(object state)
        {
            List<FileSystemEventArgs> backup = new List<FileSystemEventArgs>();

            Mutex mutex = new Mutex(false, "FSW");
            mutex.WaitOne();
            backup.AddRange(files);
            files.Clear();
            mutex.ReleaseMutex();

            foreach (FileSystemEventArgs e in backup)
            {
                if (e.ChangeType == WatcherChangeTypes.Renamed)
                {

                    reHandler(this, e as RenamedEventArgs);
                }
                else
                {
                    fswHandler(this, e);
                }
            }
        }
    }
}
