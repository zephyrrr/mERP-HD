# -*- coding: UTF-8 -*- 
import clr
clr.AddReference("System.Windows.Forms");
clr.AddReference("Feng.Base")
clr.AddReference("Feng.Controller")
clr.AddReference("Feng.Windows.Controller")
clr.AddReference("Feng.Grid");
clr.AddReference("Feng.Windows.Forms");
import System;
import Feng;

#Feng.MessageForm.ShowWarning(__name__, "__name__");

class ImeModeDataControlCollection(Feng.Windows.Collections.DataControlCollection):
	def Add(self, item):
	     Feng.Windows.Collections.DataControlCollection.Add(self, item);
	     if (isinstance(item.Control, Feng.Windows.Forms.MyComboBox)):
                 #Feng.MessageForm.ShowWarning(item.ToString());
                 #Ĭ�ϵ�ǰ���뷨�Ǹ���ϵͳ���õ�
                 item.ImeMode = System.Windows.Forms.ImeMode.Off;
	    
class ImeModeControlFactoryWindows(Feng.Windows.Forms.ControlCollectionFactoryWindows): 
    def CreateDataControlCollection(self, dm):
            ret = ImeModeDataControlCollection();
            ret.ParentManager = dm;
            return ret;

if __name__ == "<module>" or __name__ == "__builtin__" or __name__ == "__main__":
	#execute(mdiForm);
	System.Windows.Forms.MessageBox.Show("ImeMode!");
	Feng.ServiceProvider.SetDefaultService[Feng.IControlCollectionFactory](ImeModeControlFactoryWindows());
	

