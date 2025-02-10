using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LianLianKan
{
    public class LLKToken : INotifyPropertyChanged
    {
        public LLKToken(LLKTokenType content, Coordinate coordinate)
        {
            _tokenType = content;
            _isSelected = false;
            _coordinate = coordinate;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<TokenMatchedEventArgs> Matched;

        public event EventHandler<TokenSelectedEventArgs> Selected;

        public event EventHandler<TokenResetedEventArgs> Reseted;

        public Coordinate Coordinate
        {
            get
            {
                return _coordinate;
            }
        }

        public LLKTokenType TokenType
        {
            get
            {
                return _tokenType;
            }
            set
            {
                _tokenType = value;
                OnPropertyChanged(nameof(TokenType));
            }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public void OnMatched()
        {
            Matched?.Invoke(this, new TokenMatchedEventArgs(_tokenType));
        }

        public void OnMatched(TokenMatchedEventArgs e)
        {
            Matched?.Invoke(this, e);
        }

        public void OnReset()
        {
            Reseted?.Invoke(this, new TokenResetedEventArgs());
        }

        public void OnSelected()
        {
            Selected?.Invoke(this, new TokenSelectedEventArgs(_tokenType));
        }

        public static bool IsSameType(LLKToken left, LLKToken right)
        {
            return left._tokenType == right._tokenType;
        }

        public override string ToString()
        {
            if (_tokenType == LLKTokenType.None)
            {
                return "?";
            }
            return _tokenType.ToString();
        }

        #region NonPublic
        private readonly Coordinate _coordinate;
        private LLKTokenType _tokenType;
        private bool _isSelected;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public static class LinkGameHelper
    {
        public static bool TryConnect(
            List<LLKToken> gameBlocks,
            Coordinate start,
            Coordinate target,
            out Coordinate[] nodes)
        {
            var accessibleGrid = new AccessibleGrid();
            foreach (LLKToken token in gameBlocks)
            {
                accessibleGrid[token.Coordinate] = token.TokenType switch
                {
                    LLKTokenType.None => true,
                    _ => false,
                };
            }

            return TryConnectCore(accessibleGrid, start, target, out nodes);
        }

        #region NonPublic
        private sealed class AccessibleGrid
        {
            public bool this[Coordinate coordinate]
            {
                get => _accessibleMap[coordinate];
                set => _accessibleMap[coordinate] = value;
            }

            public IEnumerable<Coordinate> GetCoordinates()
            {
                return _accessibleMap.Keys;
            }

            #region NonPublic
            private readonly Dictionary<Coordinate, bool> _accessibleMap = new();
            #endregion
        }
        private static bool TryConnectCore(AccessibleGrid accessibleMap, Coordinate start, Coordinate target, out Coordinate[] nodes)
        {
            // 无转向连通检查
            if (TryZeroTurningLink(start, target, out nodes))
            {
                return true;
            }
            // 一次转向连通检查
            else if (TryOneTurningLink(start, target, out nodes))
            {
                return true;
            }
            // 二次转向连通检查
            else if (TryTwoTurningLink(start, target, out nodes))
            {
                return true;
            }
            else
            {
                nodes = Array.Empty<Coordinate>();
                return false;
            }

            // 测试纵向链接
            bool IsVLinked(Coordinate start, Coordinate target)
            {
                if (start.X != target.X)
                {
                    return false;
                }
                if (start == target)
                {
                    return true;
                }

                if (start.Y < target.Y)
                {
                    Coordinate testCoord = start.Up;
                    while (testCoord.Y < target.Y)
                    {
                        if (!accessibleMap[testCoord])
                        {
                            return false;
                        }
                        testCoord = testCoord.Up;
                    }
                }
                else
                {
                    Coordinate testCoord = start.Down;
                    while (testCoord.Y > target.Y)
                    {
                        if (!accessibleMap[testCoord])
                        {
                            return false;
                        }
                        testCoord = testCoord.Down;
                    }
                }
                return true;
            }
            // 测试横向链接
            bool IsHLinked(Coordinate start, Coordinate target)
            {
                if (start.Y != target.Y)
                {
                    return false;
                }
                if (start == target)
                {
                    return true;
                }

                if (start.X < target.X)
                {
                    Coordinate testCoord = start.Right;
                    while (testCoord.X < target.X)
                    {
                        if (!accessibleMap[testCoord])
                        {
                            return false;
                        }
                        testCoord = testCoord.Right;
                    }
                }
                else
                {
                    Coordinate testCoord = start.Left;
                    while (testCoord.X > target.X)
                    {
                        if (!accessibleMap[testCoord])
                        {
                            return false;
                        }
                        testCoord = testCoord.Left;
                    }
                }
                return true;
            }
            // 0转链接，只要纵向或者横向可以连接即可
            bool TryZeroTurningLink(Coordinate start, Coordinate target, out Coordinate[] nodes)
            {
                if (IsVLinked(start, target) || IsHLinked(start, target))
                {
                    nodes = new Coordinate[] { start, target };
                }
                else
                {
                    nodes = Array.Empty<Coordinate>();
                }

                return nodes.Length != 0;
            }
            // 1转链接，测试两点的两个纵横交点是否存在一个点可以联通
            bool TryOneTurningLink(Coordinate start, Coordinate target, out Coordinate[] nodes)
            {
                // 第一交点：横轴坐标为起点横轴坐标，纵轴坐标为目标点纵轴坐标
                var cross1 = new Coordinate(start.X, target.Y);
                // 第二交点：横轴坐标为目标点横轴坐标，纵轴坐标为起点点纵轴坐标
                var cross2 = new Coordinate(target.X, start.Y);
                // 测试第一交点连通性：检查起点与交点的纵向连通性+交点与目标点的横向连通性
                if (accessibleMap[cross1] && IsVLinked(start, cross1) && IsHLinked(cross1, target))
                {
                    nodes = new Coordinate[] { start, cross1, target };
                }
                // 测试第二交点连通性：检查起点与交点的横向连通性+交点与目标点的纵向连通性
                else if (accessibleMap[cross2] && IsHLinked(start, cross2) && IsVLinked(cross2, target))
                {
                    nodes = new Coordinate[] { start, cross2, target };
                }
                else
                {
                    nodes = Array.Empty<Coordinate>();
                }

                return nodes.Length != 0;
            }
            // 2转链接，遍历查找可以同时与start点和target点进行1转链接的点，若有多个点，则取总距离最短的点
            bool TryTwoTurningLink(Coordinate start, Coordinate target, out Coordinate[] nodes)
            {
                float minDist = -1;
                nodes = Array.Empty<Coordinate>();
                foreach (Coordinate coord in accessibleMap.GetCoordinates())
                {
                    if (!accessibleMap[coord])
                    {
                        continue;
                    }

                    // 如果解存在，首先验证当前坐标的距离合是否比当前最优解更优化
                    float dist = Coordinate.SqrDistance(start, coord) + Coordinate.SqrDistance(coord, target);
                    if (nodes.Length > 0 && dist > minDist)
                    {
                        continue;
                    }

                    if (TryOneTurningLink(start, coord, out Coordinate[] nodes1)
                        && TryOneTurningLink(coord, target, out Coordinate[] nodes2)
                        && TryZeroTurningLink(nodes1[1], nodes2[1], out _))
                    {
                        minDist = dist;
                        nodes = new Coordinate[] { start, nodes1[1], nodes2[1], target };
                    }
                }

                return nodes.Length != 0;
            }
        }
        #endregion
    }
}