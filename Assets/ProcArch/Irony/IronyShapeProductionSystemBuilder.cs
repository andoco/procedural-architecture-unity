namespace Andoco.Unity.ProcArch.Irony
{
    using System;
    using System.Text;
    using UnityEngine;
    using global::Irony.Parsing;
    using Andoco.Unity.Framework.Core.Logging;
    using Andoco.Unity.ProcArch.Shapes;
    
    public class IronyShapeProductionSystemBuilder : IShapeProductionSystemBuilder
    {
        private static readonly ILog log = LogManager.GetCurrentClassLogger ();
    
        public IShapeProductionSystem Build (string source)
        {
            var system = new ShapeProductionSystem ();
            var evaluator = new IronyArchitectureEvaluator (system);
            var grammar = new IronyArchitectureGrammar ();
            var root = GetRoot (source, grammar);
    
            var sb = new StringBuilder ();
            DispTree (root, 0, sb);
            log.Trace (sb.ToString ());
    
            evaluator.Evaluate (root);
    
            return system;
        }
    
        private static ParseTreeNode GetRoot (string sourceCode, Grammar grammar)
        {
            LanguageData language = new LanguageData (grammar);
            Parser parser = new Parser (language);
            ParseTree parseTree = parser.Parse (sourceCode);
            ParseTreeNode root = parseTree.Root;
    
            if (language.Errors.Count > 0) {
                foreach (var error in language.Errors)
                    log.Error (string.Format ("GRAMMAR: {0}", error.Message));
            }
    
            if (parseTree.HasErrors ()) {
                foreach (var msg in parseTree.ParserMessages) {
                    log.Error (string.Format ("PARSER: {0} {1}", msg.Message, msg.Location));
                }
            }
            
            if (root == null)
                throw new InvalidOperationException ("Failed to parse tree for source");
            
            return root;
        }
        
        private static void DispTree (ParseTreeNode node, int level, StringBuilder sb)
        {
            for (int i = 0; i < level; i++)
                sb.Append ("  ");
            sb.AppendLine (node.ToString ());
            
            foreach (ParseTreeNode child in node.ChildNodes)
                DispTree (child, level + 1, sb);
            
        }
    }
}