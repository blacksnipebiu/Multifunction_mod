namespace Multifunction_mod.GameModify
{
    /// <summary>
    /// 戴森球相关的游戏修改：瞬间完成戴森壳。
    /// </summary>
    public static class DysonModify
    {
        /// <summary>
        /// 瞬间完成当前星系的戴森球（节点/框架/壳全部直接补满）
        /// </summary>
        public static void FinishDysonShell()
        {
            if (GameMain.data == null || GameMain.data.dysonSpheres == null)
            {
                return;
            }

            foreach (DysonSphere ds in GameMain.data.dysonSpheres)
            {
                if (ds == null || ds.starData != GameMain.localStar) continue;
                foreach (DysonSphereLayer w in ds.layersIdBased)
                {
                    if (w == null) continue;
                    foreach (DysonNode t in w.nodePool)
                    {
                        if (t == null) continue;
                        if (t.totalSp < t.totalSpMax)
                        {
                            t.sp = 30;
                            for (int index = 0; index < t.frames.Count; ++index)
                            {
                                if (t.frames[index].nodeA == t)
                                    t.frames[index].spA = t.frames[index].spMax / 2;
                                else if (t.frames[index].nodeB == t)
                                    t.frames[index].spB = t.frames[index].spMax / 2;
                            }
                        }
                        if (t.totalCp < t.totalCpMax)
                        {
                            for (int index = 0; index < t.shells.Count; index++)
                            {
                                DysonShell shell = t.shells[index];
                                int nodeIndex = shell.nodeIndexMap[t.id];
                                shell.nodecps[nodeIndex] += (shell.vertsqOffset[nodeIndex + 1] - shell.vertsqOffset[nodeIndex]) * shell.cpPerVertex;
                                shell.nodecps[shell.nodecps.Length - 1] += (shell.vertsqOffset[nodeIndex + 1] - shell.vertsqOffset[nodeIndex]) * shell.cpPerVertex;
                            }
                        }
                    }
                }
                ds.modelRenderer.RebuildModels();
            }
        }
    }
}
