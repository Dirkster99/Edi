﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Search
{
	class SearchResultBackgroundRenderer : IBackgroundRenderer
	{
	    public TextSegmentCollection<SearchResult> CurrentResults { get; } = new TextSegmentCollection<SearchResult>();

	    public KnownLayer Layer => KnownLayer.Selection;

	    public SearchResultBackgroundRenderer()
		{
			markerBrush = Brushes.LightGreen;
			markerPen = new Pen(markerBrush, 1);
		}
		
		Brush markerBrush;
		Pen markerPen;
		
		public Brush MarkerBrush {
			get => markerBrush;
		    set {
				markerBrush = value;
				markerPen = new Pen(markerBrush, 1);
			}
		}
		
		public void Draw(TextView textView, DrawingContext drawingContext)
		{
			if (textView == null)
				throw new ArgumentNullException(nameof(textView));
			if (drawingContext == null)
				throw new ArgumentNullException(nameof(drawingContext));
			
			if (CurrentResults == null || !textView.VisualLinesValid)
				return;
			
			var visualLines = textView.VisualLines;
			if (visualLines.Count == 0)
				return;
			
			int viewStart = visualLines.First().FirstDocumentLine.Offset;
			int viewEnd = visualLines.Last().LastDocumentLine.EndOffset;
			
			foreach (SearchResult result in CurrentResults.FindOverlappingSegments(viewStart, viewEnd - viewStart)) {
                BackgroundGeometryBuilder geoBuilder = new BackgroundGeometryBuilder
                {
                    AlignToWholePixels = true,
                    BorderThickness = markerPen != null ? markerPen.Thickness : 0,
                    CornerRadius = 3
                };
                geoBuilder.AddSegment(textView, result);
				Geometry geometry = geoBuilder.CreateGeometry();
				if (geometry != null) {
					drawingContext.DrawGeometry(markerBrush, markerPen, geometry);
				}
			}
		}
	}
}
