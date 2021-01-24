using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalkerSim
{
	class BiomeData
	{
		private DictionarySave<System.String, BiomeSpawnEntityGroupList> list = new DictionarySave<string, BiomeSpawnEntityGroupList>();

		public void Init(bool clearOriginal = false)
		{
			var world = GameManager.Instance.World;

			HashSet<string> entityClassNames = new HashSet<string>();
			foreach (var pair in world.Biomes.GetBiomeMap())
			{
				var biome = pair.Value;

				BiomeSpawnEntityGroupList biomeSpawnEntityGroupList = BiomeSpawningClass.list[biome.m_sBiomeName];
				if (biomeSpawnEntityGroupList == null)
				{
					continue;
				}

                // Clearing the biome data prevents random zombie spawns
                // We make a copy to keep for ourselves for the simulation.
                var copy = new BiomeSpawnEntityGroupList {
                    list = new List<BiomeSpawnEntityGroupData>(biomeSpawnEntityGroupList.list)
                };

                if (clearOriginal)
				{
					biomeSpawnEntityGroupList.list.Clear();
				}

				Log.Out("[WalkerSim] Biome: {0}, Adding Entity Groups: {1}", biome.m_sBiomeName, string.Join(", ", copy.list.ConvertAll(e => e.entityGroupRefName)));
                foreach (var egName in copy.list.ConvertAll(e => e.entityGroupRefName)) {
					foreach (var id in EntityGroups.list[egName].ConvertAll(e => e.entityClassId)) {
						entityClassNames.Add(EntityClass.list[id].entityClassName);
					}
				}
				
					
				list.Add(biome.m_sBiomeName, copy);
			}
			Log.Out("[WalkerSim] Spawnable Entity Class Names: {0}", string.Join(", ", entityClassNames));
		}

		public int GetZombieClass(World world, Chunk chunk, int x, int y, PRNG prng)
		{
			ChunkAreaBiomeSpawnData spawnData = chunk.GetChunkBiomeSpawnData();
			if (spawnData == null)
			{
#if DEBUG
				Log.Out("No biome spawn data present");
#endif
				return -1;
			}

			var biomeData = world.Biomes.GetBiome(spawnData.biomeId);
			if (biomeData == null)
			{
#if DEBUG
				Log.Out("No biome data for biome id {0}", spawnData.biomeId);
#endif
				return -1;
			}

			BiomeSpawnEntityGroupList biomeSpawnEntityGroupList = list[biomeData.m_sBiomeName];
			if (biomeSpawnEntityGroupList == null)
			{
#if DEBUG
				Log.Out("No biome spawn group specified for {0}", biomeData.m_sBiomeName);
#endif
				return -1;
			}

			var numGroups = biomeSpawnEntityGroupList.list.Count;
			if (numGroups == 0)
			{
#if DEBUG
				Log.Out("No biome spawn group is empty for {0}", biomeData.m_sBiomeName);
#endif
				return -1;
			}

			var dayTime = world.IsDaytime() ? EDaytime.Day : EDaytime.Night;
			for (int i = 0; i < 5; i++)
			{
				int pickIndex = prng.Get(0, numGroups);

				var pick = biomeSpawnEntityGroupList.list[pickIndex];
				if (pick.daytime == EDaytime.Any || pick.daytime == dayTime)
				{
					int lastClassId = -1;
					return EntityGroups.GetRandomFromGroup(pick.entityGroupRefName, ref lastClassId);
				}
			}

			return -1;
		}
	}
}
