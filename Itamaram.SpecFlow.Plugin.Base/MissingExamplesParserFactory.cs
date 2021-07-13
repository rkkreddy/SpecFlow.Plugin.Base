using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Gherkin;
using TechTalk.SpecFlow.Parser;

namespace Itamaram.SpecFlow.Plugin.Base
{
    public class MissingExamplesParserFactory : IGherkinParserFactory
    {
        public IGherkinParser Create(IGherkinDialectProvider dialectProvider)
            => new MissingExamplesAllowingParser(dialectProvider);

        public IGherkinParser Create(CultureInfo cultureInfo)
            => new MissingExamplesAllowingParser(cultureInfo);
    }

    public class MissingExamplesAllowingParser : SpecFlowGherkinParser
    {
        public MissingExamplesAllowingParser(IGherkinDialectProvider dialectProvider) : base(dialectProvider)
        {
        }

        public MissingExamplesAllowingParser(CultureInfo defaultLanguage) : base(defaultLanguage)
        {
        }

        protected override void CheckSemanticErrors(SpecFlowDocument specFlowDocument)
        {
            try
            {
                base.CheckSemanticErrors(specFlowDocument);
            }
            catch (SemanticParserException e)
            {
                if (ShouldIgnoreException(e))
                    return;

                throw;
            }
            catch (CompositeParserException e)
            {
                if(e.Errors.All(ShouldIgnoreException))
                    return;

                throw;
            }
        }

        private static bool ShouldIgnoreException(ParserException e) =>
            Regex.IsMatch(e.Message, @"^\(\d+:\d+\): Scenario Outline '.*?' has no examples defined$");
    }
}