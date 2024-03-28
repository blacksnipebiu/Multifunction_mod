using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Multifunction_mod.Models
{
    public class VeinControlProperty
    {
        private int veinLines;
        private int cuttingveinNumbers;

        private bool AddVein;
        private bool deleteVein;
        private bool nottidyvein;
        private bool changeveinpos;
        private bool getallVein;
        private bool changeveingrouppos;
        private bool changexveinspos;
        private bool coolDown;
        private VeinData pointVeinData;

        public int AddVeinMode;

        public int VeinType;
        /// <summary>
        /// 矿脉整理行数
        /// </summary>
        public int VeinLines
        {
            get => veinLines;
            set
            {
                veinLines = value;
                Multifunction.veinlines.Value = value;
            }
        }

        public bool CoolDown
        {
            get => coolDown;
            set
            {
                coolDown = value;
                if (value)
                {
                    Task.Run(() =>
                    {
                        Thread.Sleep(1000);
                        coolDown = false;
                    });
                }
            }
        }

        /// <summary>
        /// 矿脉切割数量
        /// </summary>
        public int CuttingVeinNumbers
        {
            get => cuttingveinNumbers;
            set
            {
                cuttingveinNumbers = value;
                Multifunction.CuttingVeinNumbers.Value = value;
            }
        }

        /// <summary>
        /// 是否删除矿脉
        /// </summary>
        public bool DeleteVein
        {
            get => deleteVein;
            set
            {
                deleteVein = value;
                Multifunction.deleteveinbool.Value = value;

            }
        }

        /// <summary>
        /// 修改单矿位置
        /// </summary>
        public bool Changeveinpos
        {
            get => changeveinpos;
            set
            {
                if (value)
                {
                    SetFalse();
                }
                changeveinpos = value;
            }
        }

        /// <summary>
        /// 移动矿脉
        /// </summary>
        public bool Changeveingrouppos
        {
            get => changeveingrouppos;
            set
            {
                if (value)
                {
                    SetFalse();
                }
                changeveingrouppos = value;
            }
        }

        /// <summary>
        /// 不整理矿堆
        /// </summary>
        public bool NotTidyVein
        {
            get => nottidyvein;
            set
            {
                nottidyvein = value;
            }
        }

        /// <summary>
        /// 获取所有矿
        /// </summary>
        public bool GetallVein
        {
            get => getallVein;
            set
            {
                if (value)
                {
                    SetFalse();
                }
                getallVein = value;
            }
        }

        /// <summary>
        /// 分离矿脉
        /// </summary>
        public bool Changexveinspos
        {
            get => changexveinspos;
            set
            {
                if (value)
                {
                    SetFalse();
                }
                changexveinspos = value;
            }
        }

        /// <summary>
        /// 油井添加间隔
        /// </summary>
        public bool[] OilAddIntervalBool;

        public float[] OilAddIntervalValue;

        public float OilAddInterval;

        public int Selectedveintype;
        public int AddVeinNumber;

        public VeinData PointVeinData
        {
            get => pointVeinData;
            set
            {
                pointVeinData = value;
            }
        }

        public void SetFalse()
        {
            VeinType = 0;
            Changeveinpos = false;
            Changeveingrouppos = false;
            GetallVein = false;
            Changexveinspos = false;
        }

        public int oillowerlimit;

        public VeinControlProperty()
        {
            OilAddIntervalBool = new bool[3];
            OilAddIntervalValue = new float[3] { 0.1f, 1, 10 };
            SetOilAddInterval(1);
            AddVeinNumber = 1000000000;
        }

        internal void SetOilAddInterval(int selectedIndex)
        {
            OilAddInterval = OilAddIntervalValue[selectedIndex];
            for (int i = 0; i < 3; i++)
            {
                OilAddIntervalBool[i] = i == selectedIndex;
            }
        }


        #region 矿脉管理
        public void ControlVein()
        {
            if (VeinType != 0 && !GameMain.mainPlayer.controller.actionBuild.dismantleTool.active)
            {
                if (AddVeinMode == 0 && Input.GetMouseButtonDown(0))
                {
                    addvein(VeinType);
                }
                else if (AddVeinMode == 1 && Input.GetMouseButton(0))
                {
                    addvein(VeinType);
                }
            }
            if (Input.GetMouseButton(0) && DeleteVein && GameMain.mainPlayer.controller.actionBuild.dismantleTool.active)
            {
                removevein();
            }
            if (Input.GetMouseButton(1))
            {
                SetFalse();
            }
            if (Input.GetMouseButton(0) && !GameMain.mainPlayer.controller.actionBuild.dismantleTool.active)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    PointVeinData = getveinbymouse();
                }
                if (PointVeinData.amount != 0)
                {
                    if (GetallVein)
                    {
                        GetAllVeinMethod(PointVeinData);
                    }
                    else if (Changeveingrouppos)
                    {
                        ChangeveingroupposMethod(PointVeinData);
                    }
                    else if (Changexveinspos)
                    {
                        changexveins(PointVeinData);
                    }
                    else if (Changeveinpos)
                    {
                        if (!Physics.Raycast(GameMain.mainPlayer.controller.mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit1, 800f, 8720, (QueryTriggerInteraction)2))
                            return;
                        Vector3 raycastpos = raycastHit1.point;
                        ChangeveinposMethod(PointVeinData, raycastpos);
                    }
                }
            }
        }

        /// <summary>
        /// 添加矿脉
        /// </summary>
        /// <param name="veintype"></param>
        /// <param name="number"></param>
        /// <param name="pos"></param>
        public void addvein(int veintype)
        {
            PlanetData pd = GameMain.localPlanet;
            if (pd == null || pd.type == EPlanetType.Gas) return;
            if (!Physics.Raycast(GameMain.mainPlayer.controller.mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit1, 800f, 8720, (QueryTriggerInteraction)2))
                return;
            Vector3 raycastpos = raycastHit1.point;
            int addnum = veintype == 7 ? (int)(OilAddInterval * oillowerlimit) : AddVeinNumber;
            foreach (VeinData i in pd.factory.veinPool)
            {
                if (i.type == EVeinType.None) continue;
                if ((raycastpos - i.pos).magnitude < 1)
                {
                    if (i.type != EVeinType.Oil)
                    {
                        if (pd.factory.veinPool[i.id].amount + addnum < 1000000000)
                        {
                            pd.factory.veinPool[i.id].amount += addnum;
                            pd.factory.veinGroups[i.groupIndex].amount += addnum;
                        }
                        else
                        {
                            pd.factory.veinGroups[i.groupIndex].amount += 1000000000 - pd.factory.veinPool[i.id].amount;
                            pd.factory.veinPool[i.id].amount = 1000000000;
                        }
                    }
                    else
                    {
                        pd.factory.veinPool[i.id].amount += addnum;
                        pd.factory.veinGroups[i.groupIndex].amount += addnum;
                    }
                    return;
                }
            }
            VeinData vein = new VeinData()
            {
                amount = addnum,
                type = (EVeinType)veintype,
                pos = raycastpos,
                productId = LDB.veins.Select(veintype).MiningItem,
                modelIndex = (short)LDB.veins.Select(veintype).ModelIndex
            };
            vein.id = pd.factory.AddVeinData(vein);
            vein.colliderId = pd.physics.AddColliderData(LDB.veins.Select(veintype).prefabDesc.colliders[0].BindToObject(vein.id, 0, EObjectType.Vein, vein.pos, Quaternion.FromToRotation(Vector3.up, vein.pos.normalized)));
            vein.modelId = pd.factoryModel.gpuiManager.AddModel(vein.modelIndex, vein.id, vein.pos, Maths.SphericalRotation(vein.pos, UnityEngine.Random.value * 360f));
            vein.minerCount = 0;
            pd.factory.AssignGroupIndexForNewVein(ref vein);
            pd.factory.veinPool[vein.id] = vein;
            pd.factory.RefreshVeinMiningDisplay(vein.id, 0, 0);
            pd.factory.RecalculateVeinGroup(pd.factory.veinPool[vein.id].groupIndex);
        }

        public void changexveins(VeinData vd)
        {
            PlanetData pd = GameMain.localPlanet;
            if (pd == null || pd.type == EPlanetType.Gas) return;
            RaycastHit raycastHit1;
            if (pd == null || !Physics.Raycast(GameMain.mainPlayer.controller.mainCamera.ScreenPointToRay(Input.mousePosition), out raycastHit1, 800f, 8720, (QueryTriggerInteraction)2))
                return;
            if (pd.factory.veinGroups[pd.factory.veinPool[vd.id].groupIndex].count <= CuttingVeinNumbers)
            {
                ChangeveingroupposMethod(vd);
                return;
            }

            Vector3 raycastpos = raycastHit1.point;
            VeinData[] veinPool = pd.factory.veinPool;
            int colliderId;
            Vector3 begin = veinPool[vd.id].pos;
            bool find = false;
            List<int> veinids = new List<int>();
            foreach (VeinData vd1 in veinPool)
            {
                if (vd1.pos == null || vd1.id <= 0) continue;
                if (vd1.groupIndex == vd.groupIndex)
                {
                    int VeinId = vd1.id;
                    if (vd.id == VeinId) find = true;
                    if (!find && veinids.Count == CuttingVeinNumbers - 1) continue;
                    veinids.Add(vd1.id);
                    if (veinids.Count == CuttingVeinNumbers) break;
                }
            }
            if (veinids.Count != CuttingVeinNumbers) return;
            int index = 0;
            foreach (int VeinId in veinids)
            {
                veinPool[VeinId].pos = NotTidyVein ? raycastpos : PostionCompute(begin, raycastpos, veinPool[VeinId].pos, index++);
                if (float.IsNaN(veinPool[VeinId].pos.x) || float.IsNaN(veinPool[VeinId].pos.y) || float.IsNaN(veinPool[VeinId].pos.z))
                {
                    continue;
                }
                colliderId = veinPool[VeinId].colliderId;
                pd.physics.RemoveColliderData(colliderId);
                veinPool[VeinId].colliderId = pd.physics.AddColliderData(LDB.veins.Select((int)veinPool[VeinId].type).prefabDesc.colliders[0].BindToObject(VeinId, 0, EObjectType.Vein, veinPool[VeinId].pos, Quaternion.FromToRotation(Vector3.up, veinPool[VeinId].pos.normalized)));

                pd.factoryModel.gpuiManager.AlterModel(veinPool[VeinId].modelIndex, veinPool[VeinId].modelId, VeinId, veinPool[VeinId].pos, Maths.SphericalRotation(veinPool[VeinId].pos, 90f));

            }
            bool leave = true;
            foreach (VeinData vd1 in veinPool)
            {
                if (veinids.Contains(vd1.id) || vd1.type != vd.type)
                {
                    continue;
                }
                else if ((pd.factory.veinPool[vd.id].pos - vd1.pos).magnitude < 5)
                {
                    leave = false;
                    break;
                }
            }
            if (leave)
            {
                int origingroup = pd.factory.veinPool[vd.id].groupIndex;
                pd.factory.veinPool[vd.id].groupIndex = (short)pd.factory.AddVeinGroup(vd.type, vd.pos.normalized);
                foreach (int veinid in veinids)
                {
                    if (veinid == vd.id) continue;
                    else
                    {
                        pd.factory.veinPool[veinid].groupIndex = pd.factory.veinPool[vd.id].groupIndex;
                    }
                }
                pd.factory.RecalculateVeinGroup(pd.factory.veinPool[vd.id].groupIndex);
                pd.factory.RecalculateVeinGroup(origingroup);
                pd.factory.ArrangeVeinGroups();
            }

        }

        public void ChangeveingroupposMethod(VeinData vd)
        {
            if (CoolDown) return;
            PlanetData pd = GameMain.localPlanet;
            if (pd == null || pd.type == EPlanetType.Gas) return;
            if (!Physics.Raycast(GameMain.mainPlayer.controller.mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit1, 800f, 8720, (QueryTriggerInteraction)2))
                return;
            Vector3 raycastpos = raycastHit1.point;
            VeinData[] veinPool = pd.factory.veinPool;
            int colliderId;
            Vector3 begin = veinPool[vd.id].pos;
            int index = 0;
            foreach (VeinData vd1 in veinPool)
            {
                if (vd1.pos == null || vd1.id <= 0) continue;
                int VeinId = vd1.id;
                if (vd1.groupIndex == veinPool[vd.id].groupIndex)
                {
                    if (NotTidyVein)
                    {
                        veinPool[VeinId].pos = raycastpos;
                    }
                    else
                    {
                        Vector3 temp = PostionCompute(begin, raycastpos, vd1.pos, index++, vd.type == EVeinType.Oil);
                        if (CoolDown) return;
                        if (Vector3.Distance(temp, vd1.pos) < 0.01) continue;
                        veinPool[VeinId].pos = temp;
                        if (float.IsNaN(veinPool[VeinId].pos.x) || float.IsNaN(veinPool[VeinId].pos.y) || float.IsNaN(veinPool[VeinId].pos.z))
                        {
                            continue;
                        }
                    }
                    colliderId = veinPool[VeinId].colliderId;
                    pd.physics.RemoveColliderData(colliderId);
                    veinPool[VeinId].colliderId = pd.physics.AddColliderData(LDB.veins.Select((int)veinPool[VeinId].type).prefabDesc.colliders[0].BindToObject(VeinId, 0, EObjectType.Vein, veinPool[VeinId].pos, Quaternion.FromToRotation(Vector3.up, veinPool[VeinId].pos.normalized)));

                    pd.factoryModel.gpuiManager.AlterModel((int)veinPool[VeinId].modelIndex, veinPool[VeinId].modelId, VeinId, veinPool[VeinId].pos, Maths.SphericalRotation(veinPool[VeinId].pos, 90f));

                }
            }

            pd.factory.veinGroups[veinPool[vd.id].groupIndex].pos = veinPool[vd.id].pos / (pd.realRadius + 2.5f);
        }

        public void ChangeveinposMethod(VeinData vd, Vector3 pos)
        {
            PlanetData planet = GameMain.localPlanet;
            int VeinId = vd.id;
            if (planet == null || planet.type == EPlanetType.Gas) return;
            VeinData[] veinPool = planet.factory.veinPool;
            veinPool[VeinId].pos = pos;
            int colliderId = veinPool[VeinId].colliderId;
            planet.physics.RemoveColliderData(colliderId);
            veinPool[VeinId].colliderId = planet.physics.AddColliderData(LDB.veins.Select((int)veinPool[VeinId].type).prefabDesc.colliders[0].BindToObject(VeinId, 0, EObjectType.Vein, veinPool[VeinId].pos, Quaternion.FromToRotation(Vector3.up, veinPool[VeinId].pos.normalized)));
            planet.factoryModel.gpuiManager.AlterModel(veinPool[VeinId].modelIndex, veinPool[VeinId].modelId, VeinId, veinPool[VeinId].pos, Maths.SphericalRotation(veinPool[VeinId].pos, 90f));
            bool leave = false;
            int origingroup = -1;
            if (planet.factory.veinGroups[veinPool[VeinId].groupIndex].count > 1)
            {
                Vector3 vector3 = pos - planet.factory.veinGroups[veinPool[VeinId].groupIndex].pos * (planet.realRadius + 2.5f);
                if (vector3.magnitude > 10.0)
                {
                    leave = true;
                    origingroup = veinPool[VeinId].groupIndex;
                    veinPool[VeinId].groupIndex = -1;
                }
            }
            else
            {
                planet.factory.veinGroups[veinPool[VeinId].groupIndex].pos = veinPool[VeinId].pos / (planet.realRadius + 2.5f);
                foreach (VeinData veindata in planet.factory.veinPool)
                {
                    if (veindata.type == veinPool[VeinId].type && veindata.groupIndex != origingroup && (veindata.pos - veinPool[VeinId].pos).magnitude < 10)
                    {
                        origingroup = veinPool[VeinId].groupIndex;
                        veinPool[VeinId].groupIndex = veindata.groupIndex;
                        planet.factory.RecalculateVeinGroup(origingroup);
                    }
                }
            }
            if (leave)
            {
                planet.factory.RecalculateVeinGroup(origingroup);
                foreach (VeinData veindata in planet.factory.veinPool)
                {
                    if (veindata.type == veinPool[VeinId].type && veindata.groupIndex != origingroup && (veindata.pos - veinPool[VeinId].pos).magnitude < 10)
                    {
                        veinPool[VeinId].groupIndex = veindata.groupIndex;
                    }
                }
                if (veinPool[VeinId].groupIndex == -1)
                {
                    veinPool[VeinId].groupIndex = (short)planet.factory.AddVeinGroup(veinPool[VeinId].type, veinPool[VeinId].pos.normalized);
                }
            }
            planet.factory.RecalculateVeinGroup(veinPool[VeinId].groupIndex);
            planet.factory.ArrangeVeinGroups();
        }

        public void GetAllVeinMethod(VeinData vd)
        {
            PlanetData pd = GameMain.localPlanet;
            if (pd == null || pd.type == EPlanetType.Gas || !Physics.Raycast(GameMain.mainPlayer.controller.mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit1, 800f, 8720, (QueryTriggerInteraction)2))
                return;
            Vector3 raycastpos = raycastHit1.point;
            VeinData[] veinPool = pd.factory.veinPool;
            int colliderId;
            Vector3 begin = veinPool[vd.id].pos;
            int index = 0;
            foreach (VeinData vd1 in veinPool)
            {
                if (vd1.pos == null || vd1.id <= 0 || vd1.type != vd.type) continue;
                int VeinId = vd1.id;
                veinPool[VeinId].pos = NotTidyVein ? raycastpos : PostionCompute(begin, raycastpos, vd1.pos, index++, vd.type == EVeinType.Oil);
                if (CoolDown) return;
                if (float.IsNaN(veinPool[VeinId].pos.x) || float.IsNaN(veinPool[VeinId].pos.y) || float.IsNaN(veinPool[VeinId].pos.z))
                {
                    continue;
                }
                if (vd1.groupIndex != vd.groupIndex)
                {
                    int origingroup = veinPool[vd1.id].groupIndex;
                    veinPool[vd1.id].groupIndex = vd.groupIndex;
                    pd.factory.RecalculateVeinGroup(origingroup);
                    pd.factory.RecalculateVeinGroup(vd.groupIndex);
                    pd.factory.ArrangeVeinGroups();
                }
                colliderId = veinPool[VeinId].colliderId;
                pd.physics.RemoveColliderData(colliderId);
                veinPool[VeinId].colliderId = pd.physics.AddColliderData(LDB.veins.Select((int)veinPool[VeinId].type).prefabDesc.colliders[0].BindToObject(VeinId, 0, EObjectType.Vein, veinPool[VeinId].pos, Quaternion.FromToRotation(Vector3.up, veinPool[VeinId].pos.normalized)));

                pd.factoryModel.gpuiManager.AlterModel(veinPool[VeinId].modelIndex, veinPool[VeinId].modelId, VeinId, veinPool[VeinId].pos, Maths.SphericalRotation(veinPool[VeinId].pos, 90f));
            }
            pd.factory.RecalculateVeinGroup(vd.groupIndex);
            pd.factory.ArrangeVeinGroups();
        }

        public void removevein()
        {
            PlanetData pd = GameMain.localPlanet;
            if (pd == null || pd.type == EPlanetType.Gas || !Physics.Raycast(GameMain.mainPlayer.controller.mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit1, 800f, 8720, (QueryTriggerInteraction)2))
                return;
            Vector3 raycastpos = raycastHit1.point;
            foreach (VeinData i in pd.factory.veinPool)
            {
                if ((raycastpos - i.pos).magnitude < 1 && i.type != EVeinType.None)
                {
                    pd.factory.veinGroups[i.groupIndex].count--;
                    pd.factory.veinGroups[i.groupIndex].amount -= i.amount;
                    pd.factory.RemoveVeinWithComponents(i.id);
                    if (pd.factory.veinGroups[i.groupIndex].count == 0)
                    {
                        pd.factory.veinGroups[i.groupIndex].type = 0;
                        pd.factory.veinGroups[i.groupIndex].amount = 0;
                        pd.factory.veinGroups[i.groupIndex].pos = Vector3.zero;
                    }
                    return;
                }
            }
        }

        public VeinData getveinbymouse()
        {
            PlanetData pd = GameMain.localPlanet;
            if (pd != null && pd.type != EPlanetType.Gas && Physics.Raycast(GameMain.mainPlayer.controller.mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit1, 800f, 8720, (QueryTriggerInteraction)2))
            {
                Vector3 raycastpos = raycastHit1.point;
                float min = 100;
                VeinData vein = new VeinData();
                foreach (VeinData vd in pd.factory.veinPool)
                {
                    if (vd.id == 0) continue;
                    if ((raycastpos - vd.pos).magnitude < min && vd.type != EVeinType.None)
                    {
                        min = (raycastpos - vd.pos).magnitude;
                        vein = vd;
                    }
                }
                if (min > 4)
                {
                    return new VeinData();
                }
                else
                {
                    return vein;
                }
            }

            return new VeinData();
        }

        public Vector3 PostionCompute(Vector3 begin, Vector3 end, Vector3 pointpos, int index, bool oil = false)
        {
            if (end.y > 193 || end.y < -193)
            {
                UIMessageBox.Show("移动矿堆失败".Translate(), "当前纬度过高，为避免出错，无法移动矿堆".Translate(), "确定".Translate(), 3);
                CoolDown = true;
                return pointpos;
            }
            Vector3 pos1 = begin;
            Vector3 pos2 = end;
            Vector3 pos3;
            float radius = GameMain.localPlanet.realRadius;
            Quaternion quaternion2 = Maths.SphericalRotation(pos1, 0);
            float areaRadius = oil ? 15 : 1.5f;
            if (!oil)
            {
                pos2.x = (int)pos2.x;
                pos2.z = (int)pos2.z;
                pos2.y = (int)pos2.y;
                pos3 = pos1 + quaternion2 * (new Vector3(index / VeinLines, 0, index % VeinLines) * areaRadius);
            }
            else
                pos3 = pos1 - quaternion2 * (new Vector3((index / VeinLines) * 8, 0, index % VeinLines * areaRadius));
            double del1 = Math.Atan(pos1.z / pos1.x) - Math.Atan(pos2.z / pos2.x);
            double del2 = Math.Acos(pos1.y / radius) - Math.Acos(pos2.y / radius);
            double del3_1 = -Math.Atan(pos3.z / pos3.x) + del1;
            double del3_2 = Math.Acos(pos3.y / radius) - del2;
            if (del1 == double.NaN || del2 == double.NaN || del3_1 == double.NaN || del3_2 == double.NaN)
            {
                return pointpos;
            }
            pos3.x = (float)(end.x < 0 ? -Math.Abs(Math.Sin(del3_2) * Math.Cos(del3_1)) : Math.Abs(Math.Sin(del3_2) * Math.Cos(del3_1)));
            pos3.y = (float)(end.y < 0 ? -Math.Abs(Math.Cos(del3_2)) : Math.Abs(Math.Cos(del3_2)));
            pos3.z = (float)(end.z < 0 ? -Math.Abs(Math.Sin(del3_2) * Math.Sin(del3_1)) : Math.Abs(Math.Sin(del3_2) * Math.Sin(del3_1)));
            pos3.x *= radius;
            pos3.y *= radius;
            pos3.z *= radius;

            if (pos3.x == float.NaN || pos3.y == float.NaN || pos3.z == float.NaN || pos3.y > 190 || pos3.y < -190)
            {
                return pointpos;
            }
            return pos3;
        }

        public void BuryAllvein()
        {
            PlanetData pd = GameMain.localPlanet;
            if (pd == null || pd.type == EPlanetType.Gas) return;
            float num9 = pd.realRadius - 50f;
            foreach (VeinData i in pd.factory.veinPool)
            {
                PlanetPhysics physics = pd.physics;
                int id = i.id;
                int colliderId = i.colliderId;
                ColliderData colliderData = physics.GetColliderData(colliderId);
                Vector3 vector3_2 = colliderData.pos.normalized * (num9 + 0.4f);
                physics.colChunks[colliderId >> 20].colliderPool[colliderId & 1048575].pos = vector3_2;
                pd.factory.veinPool[id].pos = i.pos.normalized * num9;
            }
            foreach (VeinData i in pd.factory.veinPool)
            {
                GameMain.gpuiManager.AlterModel(i.modelIndex, i.modelId, i.id, i.pos, false);
            }
            GameMain.gpuiManager.SyncAllGPUBuffer();
        }

        public void RemoveAllvein()
        {
            PlanetData pd = GameMain.localPlanet;
            if (pd == null || pd.type == EPlanetType.Gas) return;
            foreach (VeinData i in pd.factory.veinPool)
            {
                if (i.type != EVeinType.None)
                {
                    pd.factory.veinGroups[i.groupIndex].count--;
                    pd.factory.veinGroups[i.groupIndex].amount -= i.amount;
                    pd.factory.RemoveVeinWithComponents(i.id);
                    if (pd.factory.veinGroups[i.groupIndex].count == 0)
                    {
                        pd.factory.veinGroups[i.groupIndex].type = 0;
                        pd.factory.veinGroups[i.groupIndex].amount = 0;
                        pd.factory.veinGroups[i.groupIndex].pos = Vector3.zero;
                    }
                }
            }
            pd.factory.ArrangeVeinGroups();
        }

        #endregion
    }
}
