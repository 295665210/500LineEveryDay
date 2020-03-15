using Autodesk.Revit.DB;
using CodeInTangsengjiewa4.BinLibrary.Extensions;
using System;


namespace CodeInTangsengjiewa4.BinLibrary.Helpers
{
    public static class TransactionHelper
    {
        public static void Invoke(this Document doc, Action<Transaction> action, string name = "Invoke")
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

#if DEBUG
            }, "C:\\revitExceptionLog.txt");
#endif
        }

        public static void Invoke(
            this Document doc, Action<Transaction> action, string name = "Invoke", bool ignoreFailure = true)
        {
            LogHelper.LogException(delegate
            {
                using (Transaction transaction = new Transaction(doc, name))
                {
                    transaction.Start();
                    if (ignoreFailure)
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
            }, "c:\\revitExceptionLog.txt");
        }

        public static void SubtranInvoke(this Document doc, Action<SubTransaction> action)
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