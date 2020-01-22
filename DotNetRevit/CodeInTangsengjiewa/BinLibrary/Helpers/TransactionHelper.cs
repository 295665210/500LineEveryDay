﻿using Autodesk.Revit.DB;
using System;
using CodeInTangsengjiewa.BinLibrary.Extensions;

namespace CodeInTangsengjiewa.BinLibrary.Helpers
{
    public static class TransactionHelper
    {
        public static void Invoke
            (this Document doc, Action<Transaction> action, string name = "Invoke")
        {
#if DEBUG
            LogHelper.LogException(delegate
            {
#endif
                using (Transaction transaction = new Transaction(doc, name))
                {
                    transaction.Start();
                    action(transaction);
                    bool flag = transaction.GetStatus() == (TransactionStatus) 1;

                    if (flag)
                    {
                        transaction.Commit();
                    }
                }
            }, "C:\\revitExceptionLog.txt");
        }

        public static void Invoke
            (this Document doc, Action<Transaction> action, string name = "Invoke", bool ignorefailure = true)
        {
            LogHelper.LogException(delegate
            {
                using (Transaction transaction = new Transaction(doc, name))
                {
                    transaction.Start();

                    if (ignorefailure)
                    {
                        transaction.IgnoreFailure();
                    }

                    action(transaction);
                    bool flag = transaction.GetStatus() == TransactionStatus.Started;

                    if (flag)
                    {
                        transaction.Commit();
                    }
                }
            }, "c:\\revitException.txt");
        }

        public static void SubtransInvoke(this Document doc, Action<SubTransaction> action)
        {
            using (SubTransaction subTransaction = new SubTransaction(doc))
            {
                subTransaction.Start();
                action(subTransaction);
                bool flag = subTransaction.GetStatus() == (TransactionStatus) 1;

                if (flag)
                {
                    subTransaction.Commit();
                }
            }
        }
    }
}