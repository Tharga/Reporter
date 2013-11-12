using System;

namespace Tharga.Reporter.Engine.Entity.Element
{
    class TableColumn
    {
        public string DisplayName { get; private set; }
        public UnitValue? Width { get; internal set; }
        public Table.WidthMode WidthMode { get; private set; }
        public Table.Alignment Align { get; private set; }
        public string HideValue { get; private set; }

        internal bool Hide { get; set; }

        public TableColumn(string displayName, UnitValue? width, Table.WidthMode widthMode, Table.Alignment align, string hideValue)
        {
            if (width == null && widthMode == Table.WidthMode.Specific) throw new InvalidOperationException("When not assigning a specific value for width the width mode cannot be set to specific.");

            DisplayName = displayName;
            Width = width;
            WidthMode = widthMode;
            Align = align;
            HideValue = hideValue;

            Hide = false;
        }
    }
}