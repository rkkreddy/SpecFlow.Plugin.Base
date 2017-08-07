using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gherkin.Ast;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Utils;

namespace Itamaram.SpecFlow.Plugin.Base
{
    public class Generator : TestGenerator
    {
        private readonly ExamplesGeneratorContainer container;

        public Generator(SpecFlowConfiguration config, ProjectSettings settings, ITestHeaderWriter writer,
            ITestUpToDateChecker checker, IFeatureGeneratorRegistry registry, CodeDomHelper dom,
            ExamplesGeneratorContainer container)
            : base(config, settings, writer, checker, registry, dom)
        {
            this.container = container;
        }

        protected override SpecFlowDocument ParseContent(SpecFlowGherkinParser parser, TextReader contentReader,
            string sourceFilePath)
        {
            var doc = base.ParseContent(parser, contentReader, sourceFilePath);
            var children = doc.SpecFlowFeature.Children.Select(c => ProcessScenarioDefinition(c, doc));
            return doc.Clone(doc.SpecFlowFeature.Clone(children: children));
        }

        private ScenarioDefinition ProcessScenarioDefinition(ScenarioDefinition definition, SpecFlowDocument doc)
        {
            if (definition is ScenarioOutline outline)
                return outline.Clone(examples: outline.Examples.Select(e => HandleTags(e, doc)));

            return definition;
        }

        private Examples HandleTags(Examples example, SpecFlowDocument doc)
        {
            if (example.Tags == null)
                return example;

            var unhandled = example.Tags.ToList();

            var header = example.TableHeader != null
                ? new ExamplesHeader(example.TableHeader.Cells.Select(c => c.Value))
                : null;

            var rows = new List<TableRow>();

            foreach (var generator in container.GetGenerators())
            {
                foreach (var tag in example.Tags)
                {
                    var name = tag.SanitiseTag();
                    if (!generator.Handles(name))
                        continue;

                    unhandled.Remove(tag);

                    rows.AddRange(generator.GetRows(name, doc.SourceFilePath, header).Select(r => r.ToTableRow(doc)));
                }
            }

            if (!rows.Any())
                return example;

            return example.TableHeader == null
                ? example.Clone(tags: unhandled, body: example.TableBody.Concat(rows.Skip(1)), header: rows.First())
                : example.Clone(tags: unhandled, body: example.TableBody.Concat(RowSorter.MaybeSortRows(example.TableHeader, rows)));
        }
    }
}