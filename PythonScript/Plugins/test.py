# -*- coding: UTF-8 -*- 
import clr
clr.AddReference("System");
clr.AddReference("Feng.Base")
clr.AddReference("Feng.Windows.Application")

import System;
import Feng;

class DemoTask(Feng.Windows.Forms.ExecuteTask):
	def DoWork(self):
		while (True):
			if (System.DateTime.Now.Second == 5):
				self.Progress(System.Array[System.String]([u'Demo', u'http://jkhd2/action/��ѯͳ��_��Ա��λ/?exp=��� = 100000&order=���&pos=1']));
			System.Threading.Thread.Sleep(1000);

def execute(mdiForm):
	return DemoTask("DemoTask");

if __name__ == "<module>" or __name__ == "__builtin__":
	result = execute(mdiForm);
	

