using System;

namespace Tharga.Reporter.Engine.Entity
{
    public class UnitRectangle
    {
        //One of the three following values can be set
        private UnitValue _top;
        private UnitValue _bottom;
        private UnitValue _height;

        //One of the three following values can be set
        private UnitValue _left;
        private UnitValue _right;
        private UnitValue _width;

        #region Constructors


        public UnitRectangle()
        {
            //Default value if not set is 0% for top, bottom, left and right. (100% for height and width)
        }

        internal UnitRectangle(UnitValue left, UnitValue top, UnitValue right, UnitValue bottom)
        {
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;
        }


        #endregion

        internal double GetTop(double totalHeight)
        {
            if (_top != null)
                return _top.GetXUnitValue(totalHeight);
            if (_bottom != null && _height != null)
                throw new NotImplementedException();
            return 0;
        }

        internal double GetBottom(double totalHeight)
        {
            if (_bottom != null)
                return _bottom.GetXUnitValue(totalHeight);
            if (_top != null && _height != null)
                return totalHeight - (_top.GetXUnitValue(totalHeight) + _height.GetXUnitValue(totalHeight));
            return 0;
        }

        internal double GetHeight(double totalHeight)
        {
            if (_height != null)
                return _height.GetXUnitValue(totalHeight);
            if (_top != null && _bottom != null)
                return totalHeight - _top.GetXUnitValue(totalHeight) - _bottom.GetXUnitValue(totalHeight);
            if (_top != null)
                return totalHeight - _top.GetXUnitValue(totalHeight);
            if (_bottom != null)
                return totalHeight - _bottom.GetXUnitValue(totalHeight);
            return totalHeight;
        }

        internal double GetLeft(double totalWidth)
        {
            if (_left != null)
                return _left.GetXUnitValue(totalWidth);
            if (_right != null && _width != null)
                return totalWidth - _width.GetXUnitValue(totalWidth) - _right.GetXUnitValue(totalWidth);
            return 0;
        }

        internal double GetRight(double totalWidth)
        {
            if (_right != null)
                return _right.GetXUnitValue(totalWidth);
            if (_left != null && _width != null)
                return totalWidth - (_left.GetXUnitValue(totalWidth) + _width.GetXUnitValue(totalWidth));
            return 0;
        }

        internal double GetWidht(double totalWidth)
        {
            if (_width != null)
                return _width.GetXUnitValue(totalWidth);
            if (_left != null && _right != null) //Left and right has been set
                return totalWidth - _left.GetXUnitValue(totalWidth) - _right.GetXUnitValue(totalWidth);
            if (_left != null) //Only left has been set
                return totalWidth - _left.GetXUnitValue(totalWidth);
            if (_right != null) //Only right has been set
                return totalWidth - _right.GetXUnitValue(totalWidth);
            return totalWidth;
        }


        public UnitValue Top
        {
            get { return _top; }
            set
            {
                if (_bottom != null && _height != null) throw new InvalidOperationException(string.Format("Only two of the values top, bottom and height can be set, the third one is calculated. Bottom ({0}) and height {1} has already been set.", _bottom.ToString(), _height.ToString()));
                _top = value;
            }
        }

        public UnitValue Bottom
        {
            get { return _bottom; }
            set
            {
                if (_top != null && _height != null) throw new InvalidOperationException(string.Format("Only two of the values top, bottom and height can be set, the third one is calculated. Top ({0}) and height {1} has already been set.", _top.ToString(), _height.ToString()));
                _bottom = value;
            }
        }

        public UnitValue Height
        {
            get { return _height; }
            set
            {
                if (_top != null && _bottom != null) throw new InvalidOperationException(string.Format("Only two of the values top, bottom and height can be set, the third one is calculated. Top ({0}) and bottom {1} has already been set.", _top.ToString(), _bottom.ToString()));
                _height = value;
            }
        }

        public UnitValue Left
        {
            get { return _left; }
            set
            {
                if (_right != null && _width != null) throw new InvalidOperationException(string.Format("Only two of the values left, right and width can be set, the third one is calculated. Right ({0}) and width {1} has already been set.", _right.ToString(), _width.ToString()));
                _left = value;
            }
        }

        public UnitValue Right
        {
            get { return _right; }
            set
            {
                if (_left != null && _width != null) throw new InvalidOperationException(string.Format("Only two of the values left, right and width can be set, the third one is calculated. Left ({0}) and width {1} has already been set.", _left.ToString(), _width.ToString()));
                _right = value;
            }
        }

        public UnitValue Width
        {
            get { return _width; }
            set
            {
                if (_left != null && _right != null) throw new InvalidOperationException(string.Format("Only two of the values left, right and width can be set, the third one is calculated. Left ({0}) and right {1} has already been set.", _left.ToString(), _right.ToString()));
                _width = value;
            }
        }
    }
}