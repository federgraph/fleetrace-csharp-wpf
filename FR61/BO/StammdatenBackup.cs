namespace RiggVar.FR
{
    public class TStammdatenBackup
    {
        TStringList FSLBackup;

        public TStammdatenBackup()
        {
            FSLBackup = new TDBStringList();
        }

        private void SaveLine(object Sender, string s)
        {
            FSLBackup.Add(s);
        }

        public void BackupStammdaten(string aFileName)
        {
            System.Diagnostics.Debug.Assert(FSLBackup != null);

            TInputAction InputAction = new TInputAction();
            InputAction.OnSend = new TInputAction.ActionEvent(SaveLine);
            TInputActionManager.DynamicActionRef = InputAction;

            TStrings oldFSBackup = TMain.BO.FSLBackup; //sollte immer Null sein

            TMain.BO.FSLBackup = FSLBackup;
            TMain.BO.BackupAthletes();
            FSLBackup.SaveToFile(aFileName);

            TMain.BO.FSLBackup = oldFSBackup;
            TInputActionManager.DynamicActionRef = null;
            FSLBackup.Clear();
        }

    }

}
