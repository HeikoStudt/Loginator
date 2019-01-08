using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Dao;

namespace Loginator.ViewModels
{
    public class DisabledNamespacesViewModel
    {
        private Dictionary<string, List<string>> DefaultDisabled = new Dictionary<string, List<string>>();
        private IConfigurationDao configurationDao;
        private object lockObject = new object();

        public DisabledNamespacesViewModel(IConfigurationDao configurationDao) {
            this.configurationDao = configurationDao;

            //TODO: consider several Loginator instances?
            var readConfig = configurationDao.Read();
            DefaultDisabled = readConfig.DeactivatedNamespaces;
        }

        internal void AddAsDefault(string application, string fullname) {
            var exeName = application.Split('(')[0];
            lock (lockObject) {
                List<string> list;
                if (!DefaultDisabled.TryGetValue(exeName, out list)) {
                    list = new List<string>();
                    DefaultDisabled.Add(exeName, list);
                }
                if (!list.Contains(fullname)) {
                    list.Add(fullname);
                }

                var readConfig = configurationDao.Read();
                readConfig.DeactivatedNamespaces = DefaultDisabled;
                configurationDao.Write(readConfig);
            }
        }

        internal IEnumerable<string> TryGetPredisabled(string application) {
            lock (lockObject) {
                var exeName = application.Split('(')[0];
                if (DefaultDisabled.TryGetValue(exeName, out List<string> disabledNamespaces)) {
                    return disabledNamespaces;
                }
                return new string[0];
            }
        }
    }
}
