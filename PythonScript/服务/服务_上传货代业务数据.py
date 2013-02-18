# -*- coding: UTF-8 -*- 
import clr
clr.AddReference("Hd.Utils")
import Hd.Utils;

def execute():
        print '正在上传货代数据.....';
        uploadCx = Hd.Utils.UploadCx("17haha8.oicp.net:8088");
        uploadCx.UpdateData();
        print '上传完毕！'
        
if __name__ == "__main__":
	execute();
if __name__ == "<module>" or __name__ == "__builtin__":
	execute();

