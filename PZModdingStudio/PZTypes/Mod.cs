namespace PZModdingStudio.PZTypes
{
    public class Mod
    {
        public Mod() { }

        private int _repeatedId = 0;
        private string _workspacePath;
        private string _workspaceStatus;

        public Mod(ModInfo modInfo) {
            this.ModInfo = modInfo;
        }

        public ModInfo ModInfo { get; set; }

        public override string ToString()
        {
            if(this._repeatedId == 0)
            {
                return ModInfo.id;
            }
            return ModInfo.id + " (" + _repeatedId + ")";
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(ModInfo.id) && !string.IsNullOrWhiteSpace(ModInfo.name);
        }

        public int SetRepeatedNumber(int number)
        {
            _repeatedId = number;
            return _repeatedId;
        }

        public int GetRepeatedId()
        {
            return _repeatedId;
        }

        public string GetWorkspacePath()
        {
            return _workspacePath;
        }

        public void SetWorkspacePath(string path)
        {
            _workspacePath = path;
        }

        public string GetWorkspaceStatus()
        {
            return _workspaceStatus;
        }

        public void SetWorkspaceStatus(string status)
        {
            _workspaceStatus = status;
        }

    }
}
