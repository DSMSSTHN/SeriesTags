using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriesTags {
    public class STOptions {
        public static ObservableCollection<MSSeries> RecentProjects { get; set; } = new ObservableCollection<MSSeries>();
        public static string AppDataPath { get; private set; } = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MSSeriesTags");
        private static string recentsFile => Path.Join(AppDataPath, "recents.txt");

        public static void LoadRecents() {
            if (!Directory.Exists(AppDataPath)) {
                Directory.CreateDirectory(AppDataPath);
                return;
            }
            if (!File.Exists(recentsFile)) return;
            var recents = File.ReadAllLines(recentsFile).Select(r => r.Trim()).Where(r => r.Length > 0).ToList();
            if (recents.Count() == 0) return;
            recents.Reverse();
            foreach (var recent in recents) {
                try { if (Directory.Exists(recent)) RecentProjects.Add(new MSSeries(recent)); } catch { }
            }
        }

        public static MSSeries AddRecent(string path) {
            var res = RecentProjects.FirstOrDefault(p => p.BasePath == path);
            if (res != null) {
                if (RecentProjects.Count > 1) RecentProjects.Remove(res);
                else return res;
            } else {
                res = new MSSeries(path);
            }
            RecentProjects.Insert(0, res);
            File.WriteAllText(recentsFile, String.Join("\n", RecentProjects.Select(p => p.BasePath)));
            return res;
        }
        public static void PutFirst(MSSeries ser) {
            if (RecentProjects.Count > 1 && RecentProjects.Contains(ser)) {
                RecentProjects.Remove(ser);
                RecentProjects.Insert(0, ser);
                File.WriteAllText(recentsFile, String.Join("\n", RecentProjects.Select(p => p.BasePath)));
            }
        }
    }
}
