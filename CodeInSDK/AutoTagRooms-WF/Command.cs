﻿//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
// 

using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using AutoTagRoomsWF;

namespace AutoTagRoomsWF
{
    /// <summary>
    /// Implements the Revit add-in interface IExternalCommand
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute
        (
            Autodesk.Revit.UI.ExternalCommandData commandData,
            ref string message,
            ElementSet elements
        )
        {
            try
            {
                //Create a transaction
                Transaction documentTransaction =
                    new Transaction(commandData.Application.ActiveUIDocument.Document, "Document");
                documentTransaction.Start();
                // Create a new instance of class RoomsData
                RoomsData data = new RoomsData(commandData);

                System.Windows.Forms.DialogResult result;

                // Create a form to display the information of rooms
                using (AutoTagRoomsForm roomsTagForm = new AutoTagRoomsForm(data))
                {
                    result = roomsTagForm.ShowDialog();
                }

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    documentTransaction.Commit();
                    return Autodesk.Revit.UI.Result.Succeeded;
                }
                else
                {
                    documentTransaction.RollBack();
                    return Autodesk.Revit.UI.Result.Cancelled;
                }
            }
            catch (Exception ex)
            {
                // If there are something wrong, give error information and return failed
                message = ex.Message;
                return Autodesk.Revit.UI.Result.Failed;
            }
        }
    }
}