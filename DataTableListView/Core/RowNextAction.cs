//-----------------------------------------------------------------------
// <copyright file="RowNextAction.cs" company="Lifeprojects.de">
//     Class: RowNextAction
//     Copyright © Lifeprojects.de 2025
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>Gerhard Ahrens@Lifeprojects.de</email>
// <date>11.08.2025 07:21:35</date>
//
// <summary>
// Enum Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace DataTableListView.Core
{
    using System;

    public enum RowNextAction : int
    {
        None = 0,
        AddRow = 1,
        UpdateRow = 2,
        CopyRow = 3,
    }
}
