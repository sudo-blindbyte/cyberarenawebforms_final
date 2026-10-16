using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CyberArenaWebForms5.Models
{
    /// <summary>Server-side SVG chart helpers (no JavaScript needed).</summary>
    public static class ChartBuilder
    {
        private static string F(double v)
        {
            return v.ToString("0.##", CultureInfo.InvariantCulture);
        }

        public static string AreaChart(string[] labels, int[] values, int max, string unit, string gradientId)
        {
            const double W = 480, H = 270;
            const double left = 40, right = 14, top = 22, bottom = 30;
            double plotW = W - left - right, plotH = H - top - bottom;

            int n = values.Length;
            var pts = new List<double[]>();
            for (int i = 0; i < n; i++)
            {
                double x = left + plotW * i / (n - 1);
                double y = top + plotH - plotH * values[i] / (double)max;
                pts.Add(new[] { x, y });
            }

            var line = new StringBuilder("M" + F(pts[0][0]) + "," + F(pts[0][1]));
            for (int i = 0; i < n - 1; i++)
            {
                double[] p0 = pts[Math.Max(i - 1, 0)], p1 = pts[i], p2 = pts[i + 1], p3 = pts[Math.Min(i + 2, n - 1)];
                double c1x = p1[0] + (p2[0] - p0[0]) / 6, c1y = p1[1] + (p2[1] - p0[1]) / 6;
                double c2x = p2[0] - (p3[0] - p1[0]) / 6, c2y = p2[1] - (p3[1] - p1[1]) / 6;
                line.Append(" C" + F(c1x) + "," + F(c1y) + " " + F(c2x) + "," + F(c2y) + " " + F(p2[0]) + "," + F(p2[1]));
            }
            string area = line + " L" + F(pts[n - 1][0]) + "," + F(top + plotH) + " L" + F(pts[0][0]) + "," + F(top + plotH) + " Z";

            var sb = new StringBuilder();
            sb.Append("<svg viewBox=\"0 0 " + F(W) + " " + F(H) + "\" xmlns=\"http://www.w3.org/2000/svg\">");
            sb.Append("<defs><linearGradient id=\"" + gradientId + "\" x1=\"0\" y1=\"0\" x2=\"0\" y2=\"1\">" +
                      "<stop offset=\"0\" stop-color=\"#3b82f6\" stop-opacity=\".22\"/>" +
                      "<stop offset=\"1\" stop-color=\"#3b82f6\" stop-opacity=\"0\"/></linearGradient></defs>");

            for (int g = 0; g <= 4; g++)
            {
                double y = top + plotH - plotH * g / 4;
                sb.Append("<line x1=\"" + F(left) + "\" x2=\"" + F(W - right) + "\" y1=\"" + F(y) + "\" y2=\"" + F(y) + "\" stroke=\"#eef1f6\"/>");
                sb.Append("<text x=\"" + F(left - 8) + "\" y=\"" + F(y + 3) + "\" text-anchor=\"end\" font-size=\"10\" fill=\"#94a3b8\">" + (int)(max * g / 4.0) + "</text>");
            }
            for (int i = 0; i < n; i++)
            {
                sb.Append("<text x=\"" + F(pts[i][0]) + "\" y=\"" + F(H - 8) + "\" text-anchor=\"middle\" font-size=\"10\" fill=\"#94a3b8\">" + System.Web.HttpUtility.HtmlEncode(labels[i]) + "</text>");
            }

            sb.Append("<path d=\"" + area + "\" fill=\"url(#" + gradientId + ")\"/>");
            sb.Append("<path d=\"" + line + "\" fill=\"none\" stroke=\"#3b82f6\" stroke-width=\"2.5\" stroke-linecap=\"round\"/>");

            double lx = pts[n - 1][0], ly = pts[n - 1][1];
            sb.Append("<circle cx=\"" + F(lx) + "\" cy=\"" + F(ly) + "\" r=\"5\" fill=\"#fff\" stroke=\"#3b82f6\" stroke-width=\"2.5\"/>");
            sb.Append("<rect x=\"" + F(lx - 62) + "\" y=\"" + F(ly - 40) + "\" width=\"70\" height=\"30\" rx=\"7\" fill=\"#0b1120\"/>");
            sb.Append("<text x=\"" + F(lx - 27) + "\" y=\"" + F(ly - 27) + "\" text-anchor=\"middle\" font-size=\"9\" fill=\"#94a3b8\">" + System.Web.HttpUtility.HtmlEncode(labels[n - 1]) + "</text>");
            sb.Append("<text x=\"" + F(lx - 27) + "\" y=\"" + F(ly - 16) + "\" text-anchor=\"middle\" font-size=\"11\" font-weight=\"700\" fill=\"#fff\">" + values[n - 1] + " " + unit + "</text>");
            sb.Append("</svg>");
            return sb.ToString();
        }
    }
}
