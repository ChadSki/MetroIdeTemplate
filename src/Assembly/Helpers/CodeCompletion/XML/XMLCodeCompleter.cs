﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Assembly.Helpers.CodeCompletion.XML
{
    public class TagCompletionEventArgs : EventArgs
    {
        public TagCompletionEventArgs(IEnumerable<CompletableXMLTag> options)
        {
            Suggestions = options;
        }

        public IEnumerable<CompletableXMLTag> Suggestions { get; private set; }
    }

    public class AttributeCompletionEventArgs : EventArgs
    {
        public AttributeCompletionEventArgs(IEnumerable<CompletableXMLAttribute> options)
        {
            Suggestions = options;
        }

        public IEnumerable<CompletableXMLAttribute> Suggestions { get; private set; }
    }

    public class ValueCompletionEventArgs : EventArgs
    {
        public ValueCompletionEventArgs(IEnumerable<CompletableXMLValue> options)
        {
            Suggestions = options;
        }

        public IEnumerable<CompletableXMLValue> Suggestions { get; private set; }
    }

    public class XMLCodeCompleter
    {
        private readonly List<CompletableXMLTag> _tags = new List<CompletableXMLTag>();

        private readonly Dictionary<string, CompletableXMLTag> _tagsByName =
            new Dictionary<string, CompletableXMLTag>();

        /// <summary>
        ///     Registers a tag for autocompletion.
        /// </summary>
        /// <param name="tag">The tag to register.</param>
        public void RegisterTag(CompletableXMLTag tag)
        {
            _tags.Add(tag);
            _tagsByName[tag.Name] = tag;
        }

        public void CompleteTag()
        {
            var args = new TagCompletionEventArgs(_tags);
            OnTagCompletionAvaiable(args);
        }

        public void CompleteAttributeName(string line, int caretColumn)
        {
            // Get information about what's being worked on
            XMLAnalysis analysis = XMLAnalysis.AnalyzeLine(line, caretColumn);
            if (!analysis.CanInsertAttribute)
                return;

            // Get the tag that's being worked on
            CompletableXMLTag tag;
            if (!_tagsByName.TryGetValue(analysis.CurrentTag, out tag))
                return;

            // Only suggest attributes which haven't been defined yet
            List<CompletableXMLAttribute> suggestions =
                tag.Attributes.Where(attribute => !analysis.Attributes.Contains(attribute.Name)).ToList();
            if (suggestions.Count == 0)
                return;

            // Raise the AttributeCompletionAvailable event
            var args = new AttributeCompletionEventArgs(suggestions);
            OnAttributeCompletionAvailable(args);
        }

        public void CompleteAttributeValue(string line, int caretColumn)
        {
            // Get information about what's being worked on
            XMLAnalysis analysis = XMLAnalysis.AnalyzeLine(line, caretColumn);
            if (analysis.CurrentAttribute == null || !analysis.IsCaretInsideAttributeValue)
                return; // Not typing out an attribute value

            // Get the tag that's being worked on
            CompletableXMLTag tag;
            if (!_tagsByName.TryGetValue(analysis.CurrentTag, out tag))
                return;

            // Get the attribute that's being worked on
            CompletableXMLAttribute attribute = tag.FindAttributeByName(analysis.CurrentAttribute);
            if (attribute == null || !attribute.HasValues)
                return;

            // Raise the ValueCompletionAvailable event
            var args = new ValueCompletionEventArgs(attribute.Values);
            OnValueCompletionAvailable(args);
        }

        protected void OnTagCompletionAvaiable(TagCompletionEventArgs args)
        {
            if (TagCompletionAvailable != null)
                TagCompletionAvailable(this, args);
        }

        protected void OnAttributeCompletionAvailable(AttributeCompletionEventArgs args)
        {
            if (AttributeCompletionAvailable != null)
                AttributeCompletionAvailable(this, args);
        }

        protected void OnValueCompletionAvailable(ValueCompletionEventArgs args)
        {
            if (ValueCompletionAvailable != null)
                ValueCompletionAvailable(this, args);
        }

        /// <summary>
        ///     Occurs when tag completion options are available and a popup should be shown.
        /// </summary>
        public event EventHandler<TagCompletionEventArgs> TagCompletionAvailable;

        /// <summary>
        ///     Occurs when tag attribute completion options are available and a popup should be shown.
        /// </summary>
        public event EventHandler<AttributeCompletionEventArgs> AttributeCompletionAvailable;

        /// <summary>
        ///     Occurs when attribute value completion options are available and a popup should be shown.
        /// </summary>
        public event EventHandler<ValueCompletionEventArgs> ValueCompletionAvailable;
    }
}