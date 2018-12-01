//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PathUtil : MonoBehaviour
//{
//    private void LeftMouseClick2(long l)
//    {
//        playerTargetPositon = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//        playerTargetPositon.z = 0;
//        playerStartPositon = transform.position;
//        playerStartPositon.z = 0;

//        Direction = Super.GetDirectionByTan(playerTargetPositon, playerStartPositon);
//        //假如点击的地点不是障碍物
//        int x1 = ((int)(playerTargetPositon.x * 100)) / ((int)(GridSize * 100));
//        int y1 = ((int)(playerTargetPositon.y * 100)) / ((int)(GridSize * 100));
//        if (Matrix[x1, y1] != 0)
//        {
//            //角色 帧动画
//            PlayerAnimation();

//            DebugDrawLines(transform.position, playerTargetPositon);

//            IsAStarMoving = false; //非寻路模式
//            MixMoveTo(playerTargetPositon); //改进型A*算法移动
//                                            //LineMoveTo(playerTargetPositon); //两点间建立直线移动
//                                            //AStarMoveTo(playerTargetPositon); //A*算法移动
//        }
//    }

















//    /// <summary>//    /// 改进型A*算法移动//    /// </summary>//    /// <param name=\"targetPoint\"></param>//private void MixMoveTo(Vector3 targetPoint)
//    {
//        //targetPoint.z = 0;
//        List<Vector3> astarTargets = new List<Vector3>();
//        int index = 0;
//        Observable.EveryUpdate().Subscribe(_ =>
//        {
//            //1、获得当前位置
//            Vector3 curenPosition = this.transform.position;

//            if (Vector3.Distance(curenPosition, targetPoint) < 0.01f)
//            {
//                transform.position = targetPoint;
//                index = 0;
//                IsAStarMoving = false;
//                // .Clear() => Dispose is called for all inner disposables, and the list is cleared.
//                // .Dispose() => Dispose is called for all inner disposables, and Dispose is called immediately after additional Adds.
//                disposables.Clear();
//            }
//            else
//            {
//                //如果不是A*寻路，且下一格子是障碍，则启动A*移动
//                if (!IsAStarMoving && WillCollide()) //false 
//                {
//                    IsAStarMoving = true;
//                    astarTargets = AStarCalc(targetPoint);

//                    DebugAStarRunRectInit(astarTargets);
//                    DebugDrawLines(playerStartPositon, transform.position);
//                    //return;
//                }

//                //距离就等于 间隔时间乘以速度即可
//                float maxDistanceDelta = Time.deltaTime * speed;
//                //如果是A*寻路
//                if (IsAStarMoving)
//                {
//                    //在寻路移动模式中，主角100%会饶过障碍物的，
//                    //因此只有在非寻路模式中才需要时时判断主角是否将要碰撞障碍物

//                    //基于MoveTowards的平移
//                    if (astarTargets.Count > 0)
//                    {
//                        //Debug.Log(string.Format(\"costTime:{0},deltaTime:{1}\", costTime, Time.deltaTime));
//                        transform.position = Vector3.MoveTowards(transform.position, astarTargets[index], maxDistanceDelta);

//                        if (Vector3.Distance(transform.position, astarTargets[index]) < 0.01f)
//                        {
//                            if (index == astarTargets.Count - 1)
//                            {
//                                index = 0;
//                                IsAStarMoving = false;
//                                transform.position = targetPoint;
//                            }
//                            else
//                            {
//                                index++;
//                            }
//                        }

//                    }
//                }
//                else
//                {

//                    Vector3 goPositon = Vector3.MoveTowards(curenPosition, targetPoint, maxDistanceDelta);
//                    transform.position = goPositon;
//                }
//            }

//        }).AddTo(disposables);
//    }

















//    /// <summary>//    /// 直线移动//    /// </summary>//    /// <param name=\"targetPoint\"></param>//private void LineMoveTo(Vector3 targetPoint)
//    {
//        Observable.EveryUpdate().Subscribe(_ =>
//        {
//            //1、获得当前位置
//            Vector3 curenPosition = this.transform.position;

//            if (Vector3.Distance(curenPosition, targetPoint) < 0.01f)
//            {
//                transform.position = targetPoint;
//                // .Clear() => Dispose is called for all inner disposables, and the list is cleared.
//                // .Dispose() => Dispose is called for all inner disposables, and Dispose is called immediately after additional Adds.
//                disposables.Clear();
//            }
//            else
//            {
//                //距离就等于 间隔时间乘以速度即可
//                float maxDistanceDelta = Time.deltaTime * speed;
//                Vector3 goPositon = Vector3.MoveTowards(curenPosition, targetPoint, maxDistanceDelta);
//                transform.position = goPositon;
//            }

//        }).AddTo(disposables);
//    }

















//    /// <summary>//    /// 判断是否将要碰撞到障碍物(障碍物预测法)//    /// </summary>//    /// <returns></returns>//private bool WillCollide()
//    {
//        //Debug.Log(\"X//Y\" +X+\"//\"+Y+ \" Matrix:\" + Matrix[X / ScreenGridSize, Y / ScreenGridSize]);
//        switch (Direction)
//        {
//            case 0:
//                return Matrix[X / ScreenGridSize, Y / ScreenGridSize + 1] == 0 ? true : false;
//            case 1:
//                return Matrix[X / ScreenGridSize + 1, Y / ScreenGridSize + 1] == 0 ? true : false;
//            case 2:
//                return Matrix[X / ScreenGridSize + 1, Y / ScreenGridSize] == 0 ? true : false;
//            case 3:
//                return Matrix[X / ScreenGridSize + 1, Y / ScreenGridSize - 1] == 0 ? true : false;
//            case 4:
//                return Matrix[X / ScreenGridSize, Y / ScreenGridSize - 1] == 0 ? true : false;
//            case 5:
//                return Matrix[X / ScreenGridSize - 1, Y / ScreenGridSize - 1] == 0 ? true : false;
//            case 6:
//                return Matrix[X / ScreenGridSize - 1, Y / ScreenGridSize] == 0 ? true : false;
//            case 7:
//                return Matrix[X / ScreenGridSize - 1, Y / ScreenGridSize + 1] == 0 ? true : false;
//            default:
//                return true;
//        }
//    }
//}
