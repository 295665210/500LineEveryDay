using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using CodeInTangsengjiewa3.BinLibrary.Extensions;

namespace CodeInTangsengjiewa3.BinLibrary.Helpers
{
    public static class TransactionHelper
    {
        public static void Invoke(this Document doc, Action<Transaction> action, string name = "Invoke")
        {
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
        }

        public static void Invoke
            (this Document doc, Action<Transaction> action, string name = "Invoke", bool ignorefailure = true)
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