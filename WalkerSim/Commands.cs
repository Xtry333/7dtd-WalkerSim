using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkerSim {
    public class CommandWalkerSim : ConsoleCmdAbstract {
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo) {
            try {
                if (_params.Count < 1)
                    return;

                var sim = API._sim;
                if (sim == null)
                    return;

                switch (_params[0]) {
                    case "respawn":
                    case "reset":
                        if (_params.Count < 2) {
                            sim.Reset();
                        } else {
                            if (bool.TryParse(_params[1], out bool flag)) {
                                sim.Reset(flag);
                            }
                        }
                        break;
                    case "pausewithoutplayers":
                        if (_params.Count < 2) {
                            Log.Out("Missing parameter: <bool>");
                        } else {
                            if (bool.TryParse(_params[1], out bool flag)) {
                                sim.SetPauseWithoutPlayers(flag);
                            }
                        }
                        break;
                    case "density":
                        if (_params.Count < 2) {
                            Log.Out("Missing parameter: <density>");
                        } else {
                            if (int.TryParse(_params[1], out int density)) {
                                sim.SetPopulationDensity(density);
                            }
                        }
                        break;
                    case "timescale":
                        if (_params.Count < 2) {
                            Log.Out("Missing parameter: <scale>");
                        } else {
                            if (float.TryParse(_params[1], out float scale)) {
                                sim.SetTimeScale(scale);
                            }
                        }
                        break;
                }
            } catch (Exception e) {
                Log.Out("Exception: {0}", e.Message);
            }
        }

        public override string[] GetCommands() {
            return new string[] { "walkersim" };
        }

        public override string GetDescription() {
            return "Walker Sim";
        }

        public override string GetHelp() {
            return "walkersim <params>\n reset => Clear and repopulate simulation\n timescale => Change timescale of simulation\n pausewithoutplayers => Change timescale of simulation";
        }
    }
}
