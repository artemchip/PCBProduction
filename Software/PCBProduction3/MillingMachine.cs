using GerberVS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PCBProduction3
{
    public class MillingMachine : CNCGeneralMachine
    {
        // Gerber
        public LibGerberVS gerberVS = new LibGerberVS();

        // Status
        private bool drillingPending = false;
        private int drillingProgressPercent = 0;
        private bool transformPending = false;
        private int transformProgressPercent = 0;
        private List<ALVertice> alVertices = new List<ALVertice>();
        private List<CHole> totalHoleList = new List<CHole>();
        private List<PointF[]> contoursToDepanelizeMm = new List<PointF[]>();
        private Vector2 dCornerTLMm = new Vector2(0.0f, 0.0f);
        private Vector2 dCornerTRMm = new Vector2(0.0f, 0.0f);
        private Vector2 dCorner3Mm = new Vector2(0.0f, 0.0f);
        private Vector2 dCorner4Mm = new Vector2(0.0f, 0.0f);

        // Отрисовка превью
        private Pen contourPen = new Pen(new SolidBrush(Color.Green));

        // Константы для CNC 3018
        private int drillingRpm = 4000;
        private int zSinkRateBigMmPerMin = 80;
        private int zRetractRateBigMmPerMin = 150;
        private int zSinkRateSmallMmPerMin = 120;
        private int zRetractRateSmallMmPerMin = 200;
        private int cuttingRpm = 7000;
        private int cuttingRateMmPerMin = 100;
        private float iCutMm = 30.0f;
        private float iGapMm = 2.5f;

        // Мои константы
        private float cuttingToolDiameterMm = 1.5f;
        private float toolChangeZMm = 38.0f;
        private float probeZMm = 18.0f;
        private float epDepthMm = 3.5f;
        private float firstPanelX = 18.0f;
        private float firstPanelY = 21.0f;
        private float secondPanelX = 146.0f;
        private float secondPanelY = 21.0f;

        public MillingMachine(String gerberFilesDir, String configFilePath, int xCamerasNeeded, int xPreviewBoxWidth, int xPreviewBoxHeight, EventHandler xStateChanged, EventHandler<ErrorEventArgs> xErrorOccurred, EventHandler<NewCameraPreviewImageArgs> xNewCameraPreviewImage, EventHandler<UserInputRequiredArgs> xUserInputRequired) : base(configFilePath, xCamerasNeeded, xPreviewBoxWidth, xPreviewBoxHeight, xStateChanged, xErrorOccurred, xNewCameraPreviewImage, xUserInputRequired)
        {
            // Константа
            calibZMm = 18.0f;
            probeZMm = calibZMm + 0.0f;
            machineIP = "";
            GerberFolderParser.LoadLayers(gerberFilesDir);
            ProcessDrills(GerberFolderParser.gbrDrillFile, GerberFolderParser.gbrProfileFile);
            ProcessIndividualCutouts(GerberFolderParser.gbrProfileFile, false, false);
        }

        private void ProcessDrills(String inputFile, String profileFile)
        {
            GerberProject proj = gerberVS.CreateNewProject();
            gerberVS.OpenLayerFromFileName(proj, inputFile);
            gerberVS.OpenLayerFromFileName(proj, profileFile);
            List<CHole> allHoles = new List<CHole>();
            Aperture[] apertures = proj.FileInfo[0].Image.ApertureArray();
            RectangleF bounds = Utils.FindBoundsOfProfileImage(proj.FileInfo[1].Image);
            foreach (GerberNet net in proj.FileInfo[0].Image.GerberNetList)
            {
                if (net.ApertureState == GerberApertureState.Flash)
                {
                    double cxInFileUnits = (net.StartX - bounds.Left);
                    double cyInFileUnits = (bounds.Bottom - net.StartY);
                    double diameterInFileUnits = apertures[net.Aperture].Parameters()[0];
                    allHoles.Add(
                        new CHole(
                            false,
                            (float)((proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (cxInFileUnits * 25.4f) : cxInFileUnits),
                            (float)((proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (cyInFileUnits * 25.4f) : cyInFileUnits),
                            (float)((proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (diameterInFileUnits * 25.4f) : diameterInFileUnits)
                        )
                    );
                }
            }

            // Stack images
            SizeF workImagePhysicalSizeInMm = new SizeF(MainForm.workImagePhysWidthMm, MainForm.workImagePhysHeightMm);
            SizeF simagePhysicalSizeInMm = new SizeF((float)((proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (bounds.Width * 25.4f) : bounds.Width), (float)((proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (bounds.Height * 25.4f) : bounds.Height));
            int possibleCols = (int)Math.Ceiling((double)workImagePhysicalSizeInMm.Width / (double)simagePhysicalSizeInMm.Width);
            int possibleRows = (int)Math.Ceiling((double)workImagePhysicalSizeInMm.Height / (double)simagePhysicalSizeInMm.Height);
            if (possibleCols < 1 || possibleRows < 1)
            {
                throw new Exception("PCB too large.");
            }
            totalHoleList.Clear();
            foreach (CHole hole in allHoles)
            {
                for (int ix = 0; ix < possibleCols; ix++)
                {
                    for (int iy = 0; iy < possibleRows; iy++)
                    {
                        CHole newHole = new CHole(
                            false,
                            MainForm.spaceOuterMm + MainForm.holderHolesIndentMm + MainForm.gapBetweenPCBsMm + hole.holeCenterMmX + ((simagePhysicalSizeInMm.Width + (MainForm.gapBetweenPCBsMm * 2.0f)) * ix),
                            MainForm.spaceOuterMm + MainForm.holderHolesIndentMm + MainForm.gapBetweenPCBsMm + hole.holeCenterMmY + ((simagePhysicalSizeInMm.Height + (MainForm.gapBetweenPCBsMm * 2.0f)) * iy),
                            (float)Math.Round(hole.diameterMm, 1)
                        );
                        if ((newHole.holeCenterMmX + hole.diameterMm) > (workImagePhysicalSizeInMm.Width + MainForm.spaceOuterMm + MainForm.holderHolesIndentMm))
                        {
                            continue;
                        }
                        if ((newHole.holeCenterMmY + hole.diameterMm) > (workImagePhysicalSizeInMm.Height + MainForm.spaceOuterMm + MainForm.holderHolesIndentMm))
                        {
                            continue;
                        }
                        totalHoleList.Add(newHole);
                    }
                }
            }

            // Calculate anchor holes
            float holderHoleRectTop = MainForm.spaceOuterMm;
            float holderHoleRectLeft = MainForm.spaceOuterMm;
            float holderHoleRectRight = MainForm.spaceOuterMm + workImagePhysicalSizeInMm.Width + (MainForm.holderHolesIndentMm * 2f);
            float holderHoleRectRightDoubleDelta = MainForm.spaceOuterMm + MainForm.workImagePhysWidthMm + (MainForm.holderHolesIndentMm * 2f);
            float holderHoleRectBottom = MainForm.spaceOuterMm + workImagePhysicalSizeInMm.Height + (MainForm.holderHolesIndentMm * 2f);
            totalHoleList.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 0.0f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 0.0f), MainForm.anchorHoleDiameterMm));
            totalHoleList.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 0.0f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 0.25f), MainForm.anchorHoleDiameterMm));
            totalHoleList.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 0.0f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 0.5f), 2.0f));
            totalHoleList.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 0.0f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 1.0f), MainForm.anchorHoleDiameterMm));
            totalHoleList.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 0.5f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 1.0f), 2.0f));
            totalHoleList.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 1.0f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 1.0f), MainForm.anchorHoleDiameterMm));
            totalHoleList.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 1.0f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 0.5f), 2.0f));
            totalHoleList.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 1.0f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 0.0f), MainForm.anchorHoleDiameterMm));
            totalHoleList.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 0.5f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 0.0f), 2.0f));
            totalHoleList.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 0.35f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 0.0f), MainForm.anchorHoleDiameterMm));
            totalHoleList.Add(new CHole(true, Utils.Lerp(holderHoleRectLeft, holderHoleRectRight, 0.2f), Utils.Lerp(holderHoleRectTop, holderHoleRectBottom, 0.0f), MainForm.anchorHoleDiameterMm));

            // Free resources
            gerberVS.UnloadAllLayers(proj);
        }

        public void ProcessIndividualCutouts(String profileFile, bool invertX, bool invertY)
        {
            // Start
            GerberProject proj = gerberVS.CreateNewProject();
            gerberVS.OpenLayerFromFileName(proj, profileFile);

            // Calculate size
            RectangleF bounds = Utils.FindBoundsOfProfileImage(proj.FileInfo[0].Image);
            SizeF singlePcbImagePhysicalSizeInMm = new SizeF((float)((proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (bounds.Width * 25.4f) : bounds.Width), (float)((proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (bounds.Height * 25.4f) : bounds.Height));

            // Find all contours
            float lastEndXInMm = -10000.0f;
            float lastEndYInMm = -10000.0f;
            List<PointF[]> singlePcbContours = new List<PointF[]>();
            List<PointF> currContour = new List<PointF>();
            foreach (GerberNet gnet in proj.FileInfo[0].Image.GerberNetList)
            {
                if (gnet.ApertureState == GerberApertureState.On)
                {
                    // File units
                    double cleanStartXInFileUnits = (invertX) ? (bounds.Width - (gnet.StartX - bounds.Left)) : (gnet.StartX - bounds.Left);
                    double cleanStartYInFileUnits = (invertY) ? (bounds.Height - (gnet.StartY - bounds.Top)) : (gnet.StartY - bounds.Top);
                    double cleanEndXInFileUnits = (invertX) ? (bounds.Width - (gnet.EndX - bounds.Left)) : (gnet.EndX - bounds.Left);
                    double cleanEndYInFileUnits = (invertY) ? (bounds.Height - (gnet.EndY - bounds.Top)) : (gnet.EndY - bounds.Top);

                    // Millimeters
                    float cstartXInMm = (float)((proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (cleanStartXInFileUnits * 25.4f) : cleanStartXInFileUnits);
                    float cstartYInMm = (float)((proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (cleanStartYInFileUnits * 25.4f) : cleanStartYInFileUnits);
                    float cendXInMm = (float)((proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (cleanEndXInFileUnits * 25.4f) : cleanEndXInFileUnits);
                    float cendYInMm = (float)((proj.FileInfo[0].Image.Unit == GerberUnit.Inch) ? (cleanEndYInFileUnits * 25.4f) : cleanEndYInFileUnits);

                    if (Math.Abs(lastEndXInMm - cstartXInMm) < 0.2f && Math.Abs(lastEndYInMm - cstartYInMm) < 0.2f)
                    {
                        // This is part of the same series of lines, just cut to endX and endY
                        currContour.Add(new PointF(cendXInMm, cendYInMm));
                    }
                    else
                    {
                        // New line, retract and slow down spindle, move to startX and startY, start and lower spindle, cut to endX and endY
                        if (currContour.Count > 0)
                        {
                            singlePcbContours.Add(currContour.ToArray());
                        }
                        currContour.Clear();
                        currContour.Add(new PointF(cstartXInMm, cstartYInMm));
                        currContour.Add(new PointF(cendXInMm, cendYInMm));
                    }

                    // Last position
                    lastEndXInMm = cendXInMm;
                    lastEndYInMm = cendYInMm;
                }
            }
            if (currContour.Count > 0)
            {
                singlePcbContours.Add(currContour.ToArray());
            }

            // Find contour areas
            List<RectangleF> contourSizes = new List<RectangleF>();
            foreach (PointF[] contour in singlePcbContours)
            {
                float minContourX = 100000.0f;
                float minContourY = 100000.0f;
                float maxContourX = 0.0f;
                float maxContourY = 0.0f;
                foreach (PointF point in contour)
                {
                    if (point.X < minContourX)
                    {
                        minContourX = point.X;
                    }
                    if (point.Y < minContourY)
                    {
                        minContourY = point.Y;
                    }
                    if (point.X > maxContourX)
                    {
                        maxContourX = point.X;
                    }
                    if (point.Y > maxContourY)
                    {
                        maxContourY = point.Y;
                    }
                }
                contourSizes.Add(new RectangleF(minContourX, minContourY, (maxContourX - minContourX), (maxContourY - minContourY)));
            }

            // Find largest contour
            RectangleF maxAreaRect = contourSizes.MaxBy((RectangleF cr) => cr.Width * cr.Height);
            int maxContourIndex = contourSizes.ToList().IndexOf(maxAreaRect);

            // Scale all contours
            for (int i = 0; i < singlePcbContours.Count; i++)
            {
                if (contourSizes[i].Width < (1.5f * MainForm.cuttingToolDiameterMm) || contourSizes[i].Height < (1.5f * MainForm.cuttingToolDiameterMm))
                {
                    singlePcbContours[i] = new PointF[0] { };
                    continue;
                }
                PointF centerPoint = new PointF(contourSizes[i].Left + (contourSizes[i].Width / 2.0f), contourSizes[i].Top + (contourSizes[i].Height / 2.0f));
                float scaleFactorX = 0.0f;
                float scaleFactorY = 0.0f;
                float delta = (maxContourIndex == i) ? MainForm.cuttingToolDiameterMm : -MainForm.cuttingToolDiameterMm;
                scaleFactorX = (contourSizes[i].Width + delta) / contourSizes[i].Width;
                scaleFactorY = (contourSizes[i].Height + delta) / contourSizes[i].Height;
                if (scaleFactorX < 0.5f && scaleFactorY < 0.5f)
                {
                    if (contourSizes[i].Width >= (1.5f * MainForm.cuttingToolDiameterMm) && contourSizes[i].Height >= (1.5f * MainForm.cuttingToolDiameterMm))
                    {
                        singlePcbContours[i] = new PointF[1] { centerPoint };
                    }
                    else
                    {
                        singlePcbContours[i] = new PointF[0] { };
                    }
                }
                else
                {
                    for (int j = 0; j < singlePcbContours[i].Length; j++)
                    {
                        singlePcbContours[i][j].X = centerPoint.X + (scaleFactorX * (singlePcbContours[i][j].X - centerPoint.X));
                        singlePcbContours[i][j].Y = centerPoint.Y + (scaleFactorY * (singlePcbContours[i][j].Y - centerPoint.Y));
                    }
                }
            }

            // Multiply by panel PCB count and sort smallest to biggest (main)
            contoursToDepanelizeMm.Clear();
            SizeF workImagePhysicalSizeInMm = new SizeF(MainForm.workImagePhysWidthMm, MainForm.workImagePhysHeightMm);
            int possibleCols = (int)Math.Floor((double)workImagePhysicalSizeInMm.Width / (double)(singlePcbImagePhysicalSizeInMm.Width + (MainForm.gapBetweenPCBsMm * 2.0f)));
            int possibleRows = (int)Math.Floor((double)workImagePhysicalSizeInMm.Height / (double)(singlePcbImagePhysicalSizeInMm.Height + (MainForm.gapBetweenPCBsMm * 2.0f)));
            if (possibleCols < 1 || possibleRows < 1)
            {
                throw new Exception("PCB too large.");
            }
            for (int ix = 0; ix < possibleCols; ix++)
            {
                for (int iy = 0; iy < possibleRows; iy++)
                {
                    foreach (PointF[] contour in singlePcbContours.Where((e) => e.Length > 0).OrderBy((e) => Utils.GetContourSize(e)))
                    {
                        List<PointF> newContour = new List<PointF>();
                        PointF? machinePosition = null;
                        foreach (PointF curPoint in contour)
                        {
                            float mPointXInMm = MainForm.spaceOuterMm + MainForm.holderHolesIndentMm + (curPoint.X + MainForm.gapBetweenPCBsMm) + (ix * (singlePcbImagePhysicalSizeInMm.Width + (MainForm.gapBetweenPCBsMm * 2.0f)));
                            float mPointYInMm = MainForm.spaceOuterMm + MainForm.holderHolesIndentMm + (curPoint.Y + MainForm.gapBetweenPCBsMm) + (iy * (singlePcbImagePhysicalSizeInMm.Height + (MainForm.gapBetweenPCBsMm * 2.0f)));
                            PointF cp = new PointF(mPointXInMm, mPointYInMm);
                            if ((machinePosition != null) && Vector2.Distance(cp.ToVector2(), (machinePosition?.ToVector2() ?? new Vector2(0f, 0f))) > 1f)
                            {
                                // Divide by pieces
                                float distTotal = Vector2.Distance(machinePosition?.ToVector2() ?? new Vector2(0f, 0f), cp.ToVector2());
                                float distPassed = 0.0f;
                                while (distPassed < distTotal)
                                {
                                    distPassed += 1f;
                                    if (distPassed > distTotal) distPassed = distTotal;
                                    Vector2 interPoint = Vector2.Lerp(machinePosition?.ToVector2() ?? new Vector2(0f, 0f), cp.ToVector2(), (distPassed / distTotal));
                                    newContour.Add(new PointF(interPoint.X, interPoint.Y));
                                }
                            }
                            else
                            {
                                // Cut
                                newContour.Add(cp);
                            }
                            machinePosition = cp;
                        }
                        contoursToDepanelizeMm.AddRange(SplitContour(newContour.ToArray()));
                    }
                }
            }
        }

        public List<PointF[]> SplitContour(PointF[] contour)
        {
            List<PointF[]> result = new List<PointF[]>();
            bool isGap = false;
            float distanceMoved = 0.0f;
            PointF? machinePosition = null;
            List<PointF> curSubcontour = new List<PointF>();
            foreach (PointF curPoint in contour)
            {
                curSubcontour.Add(curPoint);
                if (machinePosition != null)
                {
                    distanceMoved += Vector2.Distance(curPoint.ToVector2(), machinePosition?.ToVector2() ?? new Vector2(0f, 0f));
                    if (isGap && distanceMoved >= iGapMm)
                    {
                        isGap = false;
                        distanceMoved -= iGapMm;
                        curSubcontour.Clear();
                    }
                    else if (!isGap && distanceMoved >= iCutMm)
                    {
                        isGap = true;
                        distanceMoved -= iCutMm;
                        if (curSubcontour.Count > 0)
                        {
                            result.Add(curSubcontour.ToArray());
                        }
                        curSubcontour.Clear();
                    }
                }
                machinePosition = curPoint;
            }
            if (curSubcontour.Count > 0)
            {
                result.Add(curSubcontour.ToArray());
            }
            return result;
        }

        public int GetDrillingProgressPercent()
        {
            return Math.Clamp(drillingProgressPercent, 0, 100);
        }

        public int GetTransformProgressPercent()
        {
            return Math.Clamp(transformProgressPercent, 0, 100);
        }

        private List<List<CHole>> GroupHolesAndSortByMoveDistanceAndDiameter(bool includeAnchors, bool includeMain)
        {
            // Primary Filter
            List<CHole> primaryFilteredHoleList = new List<CHole>();
            for (int i = 0; i < totalHoleList.Count; i++)
            {
                if (totalHoleList[i].isAnchor)
                {
                    if (includeAnchors)
                    {
                        primaryFilteredHoleList.Add(totalHoleList[i]);
                    }
                }
                else
                {
                    if (includeMain)
                    {
                        primaryFilteredHoleList.Add(totalHoleList[i]);
                    }
                }
            }

            // Group And Sort
            List<CHole> toolList = primaryFilteredHoleList.DistinctBy((e) => e.diameterMm).OrderByDescending((e) => e.diameterMm).ToList();
            List<List<CHole>> results = new List<List<CHole>>();
            for (int i = 0; i < toolList.Count; i++)
            {
                List<CHole> sortedHoleList = Utils.SortHoles(primaryFilteredHoleList.Where((e) => e.diameterMm == toolList[i].diameterMm).ToList());
                results.Add(sortedHoleList);
            }

            // Return
            return results;
        }

        public float GetDrillingTimeSeconds(bool drillMainHoles, float zDist)
        {
            List<List<CHole>> ourHolesByTool = GroupHolesAndSortByMoveDistanceAndDiameter(true, drillMainHoles);
            if (ourHolesByTool.Count <= 0)
            {
                return 0.0f;
            }
            float totalTimeSec = 0.0f;
            float panelWidthMm = MainForm.workImagePhysWidthMm + (MainForm.spaceOuterMm + MainForm.holderHolesIndentMm) * 2f;
            float panelHeightMm = MainForm.workImagePhysHeightMm + (MainForm.spaceOuterMm + MainForm.holderHolesIndentMm) * 2f;

            // Grbl Probing + 1 Min To Attach Probe
            totalTimeSec += 60.0f;
            totalTimeSec += (((float)MillisecToExecuteMove(panelWidthMm, 0f, 0f, maxFeedRateMmPerMinX, maxFeedRateMmPerMinY, maxFeedRateMmPerMinZ)) * 2.0f) / 1000.0f;
            totalTimeSec += (((float)MillisecToExecuteMove(0f, panelHeightMm, 0f, maxFeedRateMmPerMinX, maxFeedRateMmPerMinY, maxFeedRateMmPerMinZ)) * 2.0f) / 1000.0f;

            // Drilling
            float maX = 0.0f;
            float maY = 0.0f;
            for (int i = 0; i < ourHolesByTool.Count; i++)
            {
                // 1 min to change tool
                totalTimeSec += 60.0f;

                // Each Hole
                for (int j = 0; j < ourHolesByTool[i].Count; j++)
                {
                    // Move
                    CHole thisHole = ourHolesByTool[i][j];
                    totalTimeSec += ((float)MillisecToExecuteMove(thisHole.holeCenterMmX - maX, thisHole.holeCenterMmY - maY, 0f, maxFeedRateMmPerMinX, maxFeedRateMmPerMinY, maxFeedRateMmPerMinZ)) / 1000.0f;
                    maX = thisHole.holeCenterMmX;
                    maY = thisHole.holeCenterMmY;

                    // Drill
                    if (thisHole.diameterMm >= 0.5f)
                    {
                        totalTimeSec += ((float)MillisecToExecuteMove(0f, 0f, -zDist, maxFeedRateMmPerMinX, maxFeedRateMmPerMinY, zSinkRateBigMmPerMin)) / 1000.0f;
                        totalTimeSec += ((float)MillisecToExecuteMove(0f, 0f, zDist, maxFeedRateMmPerMinX, maxFeedRateMmPerMinY, zRetractRateBigMmPerMin)) / 1000.0f;
                    } else
                    {
                        totalTimeSec += ((float)MillisecToExecuteMove(0f, 0f, -zDist, maxFeedRateMmPerMinX, maxFeedRateMmPerMinY, zSinkRateSmallMmPerMin)) / 1000.0f;
                        totalTimeSec += ((float)MillisecToExecuteMove(0f, 0f, zDist, maxFeedRateMmPerMinX, maxFeedRateMmPerMinY, zRetractRateSmallMmPerMin)) / 1000.0f;
                    }
                }
            }

            // Cutout
            totalTimeSec += 60.0f;
            totalTimeSec += (((float)MillisecToExecuteMove(panelWidthMm + cuttingToolDiameterMm, 0f, 0f, cuttingRateMmPerMin, cuttingRateMmPerMin, maxFeedRateMmPerMinZ)) * 2.0f) / 1000.0f;
            totalTimeSec += (((float)MillisecToExecuteMove(0f, panelHeightMm + cuttingToolDiameterMm, 0f, cuttingRateMmPerMin, cuttingRateMmPerMin, maxFeedRateMmPerMinZ)) * 2.0f) / 1000.0f;
            totalTimeSec += (((float)MillisecToExecuteMove(panelWidthMm, 0f, 0f, maxFeedRateMmPerMinX, maxFeedRateMmPerMinY, maxFeedRateMmPerMinZ)) * 2.0f) / 1000.0f;
            totalTimeSec += (((float)MillisecToExecuteMove(0f, panelHeightMm, 0f, maxFeedRateMmPerMinX, maxFeedRateMmPerMinY, maxFeedRateMmPerMinZ)) * 2.0f) / 1000.0f;

            // Sum
            return totalTimeSec;
        }

        public Bitmap GetGreenDepanelizeBitmap(int rDPI)
        {
            float pixelsPerMm = (((float)rDPI) / 25.4f);
            float panelSizeWidthMm = MainForm.workImagePhysWidthMm + (MainForm.spaceOuterMm + MainForm.holderHolesIndentMm) * 2f;
            float panelSizeHeightMm = MainForm.workImagePhysHeightMm + (MainForm.spaceOuterMm + MainForm.holderHolesIndentMm) * 2f;
            Bitmap bmpResult = new Bitmap((int)(panelSizeWidthMm * pixelsPerMm), (int)(panelSizeHeightMm * pixelsPerMm), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(bmpResult))
            {
                contourPen.Width = pixelsPerMm * MainForm.cuttingToolDiameterMm;
                contourPen.SetLineCap(System.Drawing.Drawing2D.LineCap.Round, System.Drawing.Drawing2D.LineCap.Round, System.Drawing.Drawing2D.DashCap.Round);
                g.Clear(Color.Black);
                if (contoursToDepanelizeMm.Count > 0)
                {
                    for (int i = 0; i < contoursToDepanelizeMm.Count; i++)
                    {
                        List<PointF> gPoints = new List<PointF>();
                        PointF prevPoint = new PointF(-1f, -1f);
                        if (contoursToDepanelizeMm[i].Length == 1)
                        {
                            float xdiameterMm = MainForm.cuttingToolDiameterMm;
                            Rectangle holeRect = new Rectangle(
                                (int)((contoursToDepanelizeMm[i][0].X * pixelsPerMm) - ((xdiameterMm * pixelsPerMm) / 2f)),
                                (int)((contoursToDepanelizeMm[i][0].Y * pixelsPerMm) - ((xdiameterMm * pixelsPerMm) / 2f)),
                                (int)(xdiameterMm * pixelsPerMm),
                                (int)(xdiameterMm * pixelsPerMm)
                            );
                            g.FillEllipse(Brushes.Yellow, holeRect);
                        }
                        else
                        {
                            for (int j = 0; j < contoursToDepanelizeMm[i].Length; j++)
                            {
                                PointF curPoint = new PointF(contoursToDepanelizeMm[i][j].X * pixelsPerMm, contoursToDepanelizeMm[i][j].Y * pixelsPerMm);
                                if (prevPoint.X != -1f && prevPoint.Y != -1f)
                                {
                                    g.DrawLine(contourPen, prevPoint, curPoint);
                                }
                                prevPoint = curPoint;
                            }
                        }
                    }
                }
            }
            return bmpResult;
        }

        public float GetTransformTimeSeconds(bool depanelize)
        {
            if (depanelize)
            {
                float totalSlowDistMm = 0.0f;
                float totalFastDistMm = 0.0f;
                float vmX = 0.0f;
                float vmY = 0.0f;
                foreach (PointF[] contour in contoursToDepanelizeMm)
                {
                    bool firstDone = false;
                    foreach (PointF p in contour)
                    {
                        float curMoveDist = (float)Math.Sqrt(Math.Pow(p.X - vmX, 2) + Math.Pow(p.Y - vmY, 2));
                        if (!firstDone)
                        {
                            totalFastDistMm += curMoveDist;
                            firstDone = true;
                        }
                        else
                        {
                            totalSlowDistMm += curMoveDist;
                        }
                        vmX = p.X;
                        vmY = p.Y;
                    }
                }
                return ((totalSlowDistMm / cuttingRateMmPerMin) + (totalFastDistMm / Math.Min(maxFeedRateMmPerMinX, maxFeedRateMmPerMinY))) * 60.0f;
            } else
            {
                List<List<CHole>> ourHolesByTool = GroupHolesAndSortByMoveDistanceAndDiameter(false, true);
                if (ourHolesByTool.Count <= 0)
                {
                    return 0.0f;
                }
                float totalTimeSec = 0.0f;
                float panelWidthMm = MainForm.workImagePhysWidthMm + (MainForm.spaceOuterMm + MainForm.holderHolesIndentMm) * 2f;
                float panelHeightMm = MainForm.workImagePhysHeightMm + (MainForm.spaceOuterMm + MainForm.holderHolesIndentMm) * 2f;

                // Drilling
                float maX = 0.0f;
                float maY = 0.0f;
                for (int i = 0; i < ourHolesByTool.Count; i++)
                {
                    // 1 min to change tool
                    totalTimeSec += 60.0f;

                    // Each Hole
                    for (int j = 0; j < ourHolesByTool[i].Count; j++)
                    {
                        // Move
                        CHole thisHole = ourHolesByTool[i][j];
                        totalTimeSec += ((float)MillisecToExecuteMove(thisHole.holeCenterMmX - maX, thisHole.holeCenterMmY - maY, 0f, maxFeedRateMmPerMinX, maxFeedRateMmPerMinY, maxFeedRateMmPerMinZ)) / 1000.0f;
                        maX = thisHole.holeCenterMmX;
                        maY = thisHole.holeCenterMmY;

                        // Drill
                        if (thisHole.diameterMm >= 0.5f)
                        {
                            totalTimeSec += ((float)MillisecToExecuteMove(0f, 0f, -epDepthMm, maxFeedRateMmPerMinX, maxFeedRateMmPerMinY, zSinkRateBigMmPerMin)) / 1000.0f;
                            totalTimeSec += ((float)MillisecToExecuteMove(0f, 0f, epDepthMm, maxFeedRateMmPerMinX, maxFeedRateMmPerMinY, zRetractRateBigMmPerMin)) / 1000.0f;
                        }
                        else
                        {
                            totalTimeSec += ((float)MillisecToExecuteMove(0f, 0f, -epDepthMm, maxFeedRateMmPerMinX, maxFeedRateMmPerMinY, zSinkRateSmallMmPerMin)) / 1000.0f;
                            totalTimeSec += ((float)MillisecToExecuteMove(0f, 0f, epDepthMm, maxFeedRateMmPerMinX, maxFeedRateMmPerMinY, zRetractRateSmallMmPerMin)) / 1000.0f;
                        }
                    }
                }
                return totalTimeSec;
            }
        }

        private bool IsPointAwayFromHolesFromBothSides(Vector2 point)
        {
            float panelWidthMm = MainForm.workImagePhysWidthMm + (MainForm.spaceOuterMm + MainForm.holderHolesIndentMm) * 2f;
            CHole closestHoleTop = totalHoleList.MinBy((e) => Vector2.Distance(point, new Vector2(e.holeCenterMmX, e.holeCenterMmY))) ?? new CHole(false, 10000f, 10000f, 1f);
            CHole closestHoleBottom = totalHoleList.MinBy((e) => Vector2.Distance(point, new Vector2(panelWidthMm - e.holeCenterMmX, e.holeCenterMmY))) ?? new CHole(false, 10000f, 10000f, 1f);
            float dist1 = Vector2.Distance(point, new Vector2(closestHoleTop.holeCenterMmX, closestHoleTop.holeCenterMmY));
            float dist2 = Vector2.Distance(point, new Vector2(closestHoleBottom.holeCenterMmX, closestHoleBottom.holeCenterMmY));
            return (dist1 > ((closestHoleTop.diameterMm + 3f) / 2f)) && (dist2 > ((closestHoleBottom.diameterMm + 3f) / 2f));
        }

        private float CalculateZTouchLevelForPointMm(float panelPointXMm, float panelPointYMm)
        {
            Vector2 panelPointVec = new Vector2(panelPointXMm, panelPointYMm);
            ALVertice? vTL = alVertices.Where((e) => e.locXMm <= panelPointXMm && e.locYMm <= panelPointYMm).MinBy((e) => Vector2.Distance(panelPointVec, new Vector2(e.locXMm, e.locYMm)));
            ALVertice? vTR = alVertices.Where((e) => e.locXMm > panelPointXMm && e.locYMm <= panelPointYMm).MinBy((e) => Vector2.Distance(panelPointVec, new Vector2(e.locXMm, e.locYMm)));
            ALVertice? vBL = alVertices.Where((e) => e.locXMm <= panelPointXMm && e.locYMm > panelPointYMm).MinBy((e) => Vector2.Distance(panelPointVec, new Vector2(e.locXMm, e.locYMm)));
            ALVertice? vBR = alVertices.Where((e) => e.locXMm > panelPointXMm && e.locYMm > panelPointYMm).MinBy((e) => Vector2.Distance(panelPointVec, new Vector2(e.locXMm, e.locYMm)));
            if (vTL == null)
            {
                return vBR!.zTouchLevelMm;
            }
            else if (vTR == null)
            {
                return vBL!.zTouchLevelMm;
            }
            else if (vBL == null)
            {
                return vTR!.zTouchLevelMm;
            }
            else if (vBR == null)
            {
                return vTL!.zTouchLevelMm;
            }
            float factorX = (panelPointXMm - vTL.locXMm) / (vBR.locXMm - vTL.locXMm);
            float factorY = (panelPointYMm - vTL.locYMm) / (vBR.locYMm - vTL.locYMm);
            if (factorX > 1.0f) factorX = 1.0f;
            if (factorX < 0.0f) factorX = 0.0f;
            if (factorY > 1.0f) factorY = 1.0f;
            if (factorY < 0.0f) factorY = 0.0f;
            float f_top = Utils.Lerp(vTL.zTouchLevelMm, vTR.zTouchLevelMm, factorX);
            float f_bottom = Utils.Lerp(vBL.zTouchLevelMm, vBR.zTouchLevelMm, factorX);
            return Utils.Lerp(f_top, f_bottom, factorY);
        }

        public float MaxZLevelMm()
        {
            return alVertices?.MaxBy((e) => e.zTouchLevelMm)?.zTouchLevelMm ?? 0f;
        }

        private void CalculateAndSetDrillingProgress(int curStep, int totalSteps, float progressOfThisStep)
        {
            float progressPercent = ((((float)curStep) * 100.0f) / ((float)totalSteps)) + ((progressOfThisStep * 100.0f) / ((float)totalSteps));
            drillingProgressPercent = (int)progressPercent;
            StateChanged.Invoke(this, new EventArgs());
        }

        private void DoDrillingProcess(bool shouldDrillMainHoles, int newPanelIdx, float alSafeHeightMm, float alWorkpieceThicknessMm)
        {
            try
            {
                // Reset
                shouldCancelOp = false;
                drillingProgressPercent = 0;
                drillingPending = true;
                SetBusyWithLongTask(true);
                StateChanged.Invoke(this, new EventArgs());

                // Grouped Holes By Tool Diameter (From Large To Small)
                List<List<CHole>> ourHolesByTool = GroupHolesAndSortByMoveDistanceAndDiameter(true, shouldDrillMainHoles);
                if (ourHolesByTool.Count <= 0)
                {
                    throw new Exception("Нечего сверлить.");
                }

                // Make sure we have required tools
                String drillBitDiameters = "";
                int i = 0;
                for (i = 0; i < ourHolesByTool.Count; i++) {
                    if (ourHolesByTool[i].Count <= 0)
                    {
                        throw new Exception("Список отверстий для одного из инструментов пуст.");
                    } else
                    {
                        drillBitDiameters += ourHolesByTool[i][0].diameterMm.ToString("0.0") + "мм";
                        if (i < (ourHolesByTool.Count - 1))
                        {
                            drillBitDiameters += ", ";
                        }
                    }
                }
                int res = AskUserForInput("Убедитесь, что у вас есть сверла диаметров: " + drillBitDiameters + " и фреза диаметром " + cuttingToolDiameterMm.ToString("0.0") + " мм.", new List<string> { "Есть", "Нет" }, 1, false, false, false);
                if (res > 0)
                {
                    throw new Exception("Операция отменена.");
                }

                // Select Panel Starting Point And Move To It
                Vector2 panelLeftTopMm;
                if (newPanelIdx == 0) {
                    panelLeftTopMm = new Vector2(firstPanelX, firstPanelY);
                } else if (newPanelIdx == 1)
                {
                    panelLeftTopMm = new Vector2(secondPanelX, secondPanelY);
                } else
                {
                    throw new Exception("Неверный индекс панели.");
                }
                MoveToAbsolutePositionAndWaitForCompletion(false, null, null, toolChangeZMm, -1.0f);
                MoveToAbsolutePositionAndWaitForCompletion(false, panelLeftTopMm.X, panelLeftTopMm.Y, null, -1.0f);
                if (shouldCancelOp)
                {
                    throw new Exception("Операция отменена.");
                }

                // Ask To Attach Grbl Probe
                res = AskUserForInput("Установите сверло 2 мм в патрон и прицепите Grbl Probe к нему для построения карты высот.", new List<string> { "Готово", "Отмена" }, 1, false, false, false);
                if (res > 0)
                {
                    throw new Exception("Операция отменена.");
                }

                // Точки для измерения
                float panelWidthMm = MainForm.workImagePhysWidthMm + (MainForm.spaceOuterMm + MainForm.holderHolesIndentMm) * 2f;
                float panelHeightMm = MainForm.workImagePhysHeightMm + (MainForm.spaceOuterMm + MainForm.holderHolesIndentMm) * 2f;
                RectangleF measureRect = new RectangleF(new PointF(2f, 2f), new SizeF(panelWidthMm - 4f, panelHeightMm - 4f));
                List<Vector2> panelPointsToMeasureMmRaw = new List<Vector2>();
                int alAccuracyPoints = 2;
                for (int ax = 0; ax < alAccuracyPoints; ax++)
                {
                    for (int ay = 0; ay < alAccuracyPoints; ay++)
                    {
                        Vector2 curPoint = new Vector2(Utils.Lerp(measureRect.Left, measureRect.Right, (((float)ax) / ((float)(alAccuracyPoints - 1)))), Utils.Lerp(measureRect.Top, measureRect.Bottom, (((float)ay) / ((float)(alAccuracyPoints - 1)))));
                        int attempts = 0;
                        while (!IsPointAwayFromHolesFromBothSides(curPoint))
                        {
                            curPoint.X += 0.1f;
                            curPoint.Y += 0.1f;
                            attempts++;
                            if (attempts > 50)
                            {
                                throw new Exception("Отверстия мешают измерить уровень в середине заготовки.");
                            }
                        }
                        panelPointsToMeasureMmRaw.Add(curPoint);
                    }
                }

                // Перемещение на высоту измерения
                MoveToAbsolutePositionAndWaitForCompletion(false, null, null, probeZMm, -1.0f);

                // Их измерение
                alVertices.Clear();
                List<Vector2> panelPointsToMeasureMm = Utils.SortPoints(panelPointsToMeasureMmRaw);
                for (i = 0; i < panelPointsToMeasureMm.Count; i++)
                {
                    // Переместиться
                    float tgtPointX = panelLeftTopMm.X + panelPointsToMeasureMm[i].X;
                    float tgtPointY = panelLeftTopMm.Y + panelPointsToMeasureMm[i].Y;
                    MoveToAbsolutePositionAndWaitForCompletion(false, tgtPointX, tgtPointY, null, -1.0f);

                    // Probe
                    float thisHeightMm = GrblProbeCurrentLocation();
                    alVertices.Add(new ALVertice(panelPointsToMeasureMm[i].X, panelPointsToMeasureMm[i].Y, thisHeightMm));

                    // Отмена и прогресс
                    CalculateAndSetDrillingProgress(0, 3, ((float)(i + 1)) / ((float)panelPointsToMeasureMm.Count));
                    if (shouldCancelOp)
                    {
                        break;
                    }
                }
                if (shouldCancelOp)
                {
                    throw new Exception("Операция отменена.");
                }

                // К началу проводящего рисунка
                CalculateAndSetDrillingProgress(1, 3, 0.0f);
                MoveToAbsolutePositionAndWaitForCompletion(false, panelLeftTopMm.X + (MainForm.spaceOuterMm + MainForm.holderHolesIndentMm), panelLeftTopMm.Y + (MainForm.spaceOuterMm + MainForm.holderHolesIndentMm), null, -1.0f);

                // Each Diameter
                i = 0;
                int holesDrilled = 0;
                int holesTotalThisRun = ourHolesByTool.Sum(o => o.Count);
                while (i < ourHolesByTool.Count)
                {
                    // Ask To Change Tool
                    MoveToAbsolutePositionAndWaitForCompletion(false, null, null, toolChangeZMm, -1.0f);
                    if (i > 0)
                    {
                        res = AskUserForInput("Если все прошло нормально, поставьте в патрон сверло диаметром " + ourHolesByTool[i].First().diameterMm.ToString("0.0") + " мм и нажмите [готово]. Если предыдущее (" + ourHolesByTool[i - 1].First().diameterMm.ToString("0.0") + " мм) сломалось - поставьте его же новую версию и нажмите [повтор].", new List<string> { "Готово", "Повтор", "Отмена" }, 1, false, false, false);
                        if (res > 1)
                        {
                            throw new Exception("Операция отменена.");
                        } else if (res > 0)
                        {
                            i--;
                        }
                    } else
                    {
                        res = AskUserForInput("Поставьте в патрон сверло диаметром " + ourHolesByTool[i].First().diameterMm.ToString("0.0") + " мм и нажмите [готово].", new List<string> { "Готово", "Отмена" }, 1, false, false, false);
                        if (res > 0)
                        {
                            throw new Exception("Операция отменена.");
                        }
                    }

                    // Tool Installed, Move To More Working Height
                    MoveToAbsolutePositionAndWaitForCompletion(false, null, null, probeZMm, -1.0f);

                    // Start spindle
                    SetSpindleRpm(drillingRpm);

                    // Drill each of holes
                    for (int j = 0; j < ourHolesByTool[i].Count; j++)
                    {
                        // Отверстие
                        CHole thisHole = ourHolesByTool[i][j];

                        // Вычислить высоты
                        float touchZMm = CalculateZTouchLevelForPointMm(thisHole.holeCenterMmX, thisHole.holeCenterMmY);
                        float zSafeMm = touchZMm + alSafeHeightMm;
                        float zWorkMm = touchZMm - alWorkpieceThicknessMm;

                        // Переместиться к отверстию и безопасной высоте для него отверстия
                        if (zSafeMm > GetAbsZMm())
                        {
                            // Безопасный Z выше, чем сейчас, сперва поднимаем Z, потом перемещаемся по XY
                            MoveToAbsolutePositionAndWaitForCompletion(false, null, null, zSafeMm, -1.0f);
                            MoveToAbsolutePositionAndWaitForCompletion(false, panelLeftTopMm.X + thisHole.holeCenterMmX, panelLeftTopMm.Y + thisHole.holeCenterMmY, null, -1.0f);
                        } else
                        {
                            // Наоборот
                            MoveToAbsolutePositionAndWaitForCompletion(false, panelLeftTopMm.X + thisHole.holeCenterMmX, panelLeftTopMm.Y + thisHole.holeCenterMmY, null, -1.0f);
                            MoveToAbsolutePositionAndWaitForCompletion(false, null, null, zSafeMm, -1.0f);
                        }

                        // Сверло вниз
                        MoveToAbsolutePositionAndWaitForCompletion(true, null, null, zWorkMm, (thisHole.diameterMm < 0.5f) ? zSinkRateSmallMmPerMin : zSinkRateBigMmPerMin);

                        // Сверло вверх
                        MoveToAbsolutePositionAndWaitForCompletion(true, null, null, zSafeMm, (thisHole.diameterMm < 0.5f) ? zRetractRateSmallMmPerMin : zRetractRateBigMmPerMin);

                        // Прогресс
                        holesDrilled++;
                        CalculateAndSetDrillingProgress(1, 3, ((float)holesDrilled) / ((float)holesTotalThisRun));

                        // Отмена
                        if (shouldCancelOp)
                        {
                            break;
                        }
                    }

                    // Stop spindle
                    SetSpindleRpm(0);

                    // Next Tool
                    i++;

                    // Move To Probe Height
                    MoveToAbsolutePositionAndWaitForCompletion(false, null, null, probeZMm, -1.0f);

                    // Ask If Last Tool Broken
                    if (i >= ourHolesByTool.Count)
                    {
                        res = AskUserForInput("Если это сверло (" + ourHolesByTool.Last().First().diameterMm.ToString("0.0") + " мм) сломалось, замените его и нажмите [повтор], в противном случае нажмите [все нормально].", new List<string> { "Все нормально", "Повтор" }, 1, false, false, false);
                        if (res > 0)
                        {
                            i--;
                        }
                    }

                    // Отмена
                    if (shouldCancelOp)
                    {
                        throw new Exception("Операция отменена.");
                    }
                }

                // Translate to Panel Start And Tool Change Height
                MoveToAbsolutePositionAndWaitForCompletion(false, panelLeftTopMm.X, panelLeftTopMm.Y, toolChangeZMm, -1.0f);

                // Ask To Install Cutting Bit
                res = AskUserForInput("Поставьте в патрон фрезу диаметром " + cuttingToolDiameterMm.ToString("0.0") + " мм и нажмите [готово].", new List<string> { "Готово", "Отмена" }, 1, false, false, false);
                if (res > 0)
                {
                    throw new Exception("Операция отменена.");
                }

                // Прямоугольник который вырезаем
                float cutGapMm = cuttingToolDiameterMm * 2f;
                float panelSizeWidthMm = MainForm.workImagePhysWidthMm + (MainForm.spaceOuterMm + MainForm.holderHolesIndentMm) * 2f;
                float panelSizeHeightMm = MainForm.workImagePhysHeightMm + (MainForm.spaceOuterMm + MainForm.holderHolesIndentMm) * 2f;
                RectangleF cuttingRect = new RectangleF(panelLeftTopMm.X - (MainForm.cuttingToolDiameterMm / 2f), panelLeftTopMm.Y - (MainForm.cuttingToolDiameterMm / 2f), panelSizeWidthMm + MainForm.cuttingToolDiameterMm, panelSizeHeightMm + MainForm.cuttingToolDiameterMm);

                // Перемещаемся в начало панели минус половина фрезы, на SafeZ по среднему уровню
                float zMaxTouchMm = MaxZLevelMm();
                if (zMaxTouchMm <= 0.0f)
                {
                    throw new Exception("Неверный Z-уровень.");
                }
                MoveToAbsolutePositionAndWaitForCompletion(false, cuttingRect.Left, cuttingRect.Top, zMaxTouchMm + alSafeHeightMm, -1.0f);
                if (shouldCancelOp)
                {
                    throw new Exception("Операция отменена.");
                }

                // Запускаем шпиндель
                SetSpindleRpm(cuttingRpm);

                // Основные резы
                float cuttingProgress = 0.0f;
                for (int nc = 0; nc < 4; nc++)
                {
                    // Опустить шпиндель
                    MoveToAbsolutePositionAndWaitForCompletion(true, null, null, zMaxTouchMm - alWorkpieceThicknessMm, zSinkRateBigMmPerMin);

                    // Рез
                    if (!shouldCancelOp)
                    {
                        if (nc == 0 || nc == 2)
                        {
                            // Рез 1/3 (X) 
                            for (int j = 0; j < 20; j++)
                            {
                                // Количество реза
                                float cutAmount = 0.0f;
                                if (nc == 0)
                                {
                                    cutAmount = (cuttingRect.Width - cutGapMm) / 20.0f;
                                }
                                else
                                {
                                    cutAmount = -(cuttingRect.Width - cutGapMm) / 20.0f;
                                }

                                // Сам рез
                                MoveRelativeAndWaitForCompletion(true, cutAmount, 0f, 0f, cuttingRateMmPerMin);

                                // Отмена
                                if (shouldCancelOp)
                                {
                                    break;
                                }

                                // Прогресс
                                cuttingProgress += 0.01f;
                                CalculateAndSetDrillingProgress(2, 3, cuttingProgress);
                            }
                        }
                        else if (nc == 1 || nc == 3)
                        {
                            // Рез 2/4 (Y)
                            for (int j = 0; j < 20; j++)
                            {
                                // Количество реза
                                float cutAmount = 0.0f;
                                if (nc == 1)
                                {
                                    cutAmount = (cuttingRect.Height - cutGapMm) / 20.0f;
                                }
                                else
                                {
                                    cutAmount = -(cuttingRect.Height - cutGapMm) / 20.0f;
                                }

                                // Сам рез
                                MoveRelativeAndWaitForCompletion(true, 0f, cutAmount, 0f, cuttingRateMmPerMin);

                                // Отмена
                                if (shouldCancelOp)
                                {
                                    break;
                                }

                                // Прогресс
                                cuttingProgress += 0.01f;
                                CalculateAndSetDrillingProgress(2, 3, cuttingProgress);
                            }
                        }
                        else
                        {
                            SetSpindleRpm(0);
                            throw new Exception("Неверный рез.");
                        }
                    }

                    // Поднять шпиндель
                    MoveToAbsolutePositionAndWaitForCompletion(true, null, null, zMaxTouchMm + alSafeHeightMm, zRetractRateBigMmPerMin);

                    // Совершена отмена?
                    if (shouldCancelOp)
                    {
                        SetSpindleRpm(0);
                        throw new Exception("Произведена отмена.");
                    }

                    // Окончание реза (gap)
                    if (nc == 0 || nc == 2)
                    {
                        MoveRelativeAndWaitForCompletion(true, (nc == 0) ? cutGapMm : -cutGapMm, 0f, 0f, -1.0f);
                    }
                    else if (nc == 1 || nc == 3)
                    {
                        MoveRelativeAndWaitForCompletion(true, 0f, (nc == 1) ? cutGapMm : -cutGapMm, 0f, -1.0f);
                    }
                    else
                    {
                        SetSpindleRpm(0);
                        throw new Exception("Неверный gap.");
                    }

                    // Совершена отмена?
                    if (shouldCancelOp)
                    {
                        SetSpindleRpm(0);
                        throw new Exception("Операция отменена.");
                    }
                }

                // Отделение панели
                Vector2[][] finalCuts = {
                    new Vector2[] { new Vector2(cuttingRect.Left, cuttingRect.Bottom), new Vector2(cuttingRect.Left + cutGapMm, cuttingRect.Bottom) },
                    new Vector2[] { new Vector2(cuttingRect.Right, cuttingRect.Bottom), new Vector2(cuttingRect.Right, cuttingRect.Bottom - cutGapMm) },
                    new Vector2[] { new Vector2(cuttingRect.Right, cuttingRect.Top), new Vector2(cuttingRect.Right - cutGapMm, cuttingRect.Top) },
                    new Vector2[] { new Vector2(cuttingRect.Left, cuttingRect.Top + cutGapMm), new Vector2(cuttingRect.Left, cuttingRect.Top) },
                };
                for (i = 0; i < finalCuts.Length; i++)
                {
                    // Переместиться к начальной точке
                    MoveToAbsolutePositionAndWaitForCompletion(false, finalCuts[i][0].X, finalCuts[i][0].Y, null, -1.0f);

                    // Ask
                    if (i == 1)
                    {
                        try
                        {
                            res = AskUserForInput("Придерживайте заготовку во время следующих резов, нажмите ОК чтобы продолжить.", new List<string> { "ОК", "Отмена" }, 1, false, false, false);
                        } catch
                        {
                            res = 1;
                        }
                        if (res > 0)
                        {
                            break;
                        }
                    }

                    // Опустить шпиндель
                    MoveToAbsolutePositionAndWaitForCompletion(true, null, null, zMaxTouchMm - alWorkpieceThicknessMm, zSinkRateBigMmPerMin);

                    // Прорезать до конечной точки
                    MoveToAbsolutePositionAndWaitForCompletion(true, finalCuts[i][1].X, finalCuts[i][1].Y, null, cuttingRateMmPerMin);

                    // Поднять шпиндель
                    MoveToAbsolutePositionAndWaitForCompletion(true, null, null, zMaxTouchMm + alSafeHeightMm, zRetractRateBigMmPerMin);

                    // Обработка отмены
                    if (shouldCancelOp)
                    {
                        break;
                    }

                    // Прогресс
                    cuttingProgress += 0.05f;
                    CalculateAndSetDrillingProgress(2, 3, cuttingProgress);
                }

                // Останавливаем шпиндель
                SetSpindleRpm(0);

                // Поднимаемся на высоту probe
                MoveToAbsolutePositionAndWaitForCompletion(false, null, null, probeZMm, -1.0f);

                // Поднимаемся на высоту смены инструмента и центр рабочего поля
                MoveToAbsolutePositionAndWaitForCompletion(false, maxTravelMmX / 2.0f, maxTravelMmY / 2.0f, toolChangeZMm, -1.0f);

                // Finale
                drillingPending = false;
                SetBusyWithLongTask(false);
                StateChanged.Invoke(this, new EventArgs());
            }
            catch (Exception exc) {
                drillingPending = false;
                SetBusyWithLongTask(false);
                ErrorOccurred.Invoke(this, new ErrorEventArgs(exc.Message));
            }
        }
        private float GetExPanelRotAngleRad()
        {
            Vector2 diff1 = Vector2.Subtract(dCornerTRMm, dCornerTLMm);
            Vector2 diff2 = Vector2.Subtract(dCorner4Mm, dCorner3Mm);
            if (dCornerTLMm.Length() > 0f && dCornerTRMm.Length() > 0f)
            {
                if (dCorner3Mm.Length() > 0f && dCorner4Mm.Length() > 0f)
                {
                    return ((float)Math.Atan(diff1.Y / diff1.X) + (float)Math.Atan(diff2.Y / diff2.X)) / 2f;
                }
                else
                {
                    return (float)Math.Atan(diff1.Y / diff1.X);
                }
            }
            else
            {
                return 0.0f;
            }
        }

        private Vector2 PanelCoordToWorkAreaCoord(Vector2 panelCoord)
        {
            if (dCorner3Mm.Length() < 1f && dCorner4Mm.Length() < 1f)
            {
                Vector2 tgtRotated = Vector2.Transform(new Vector2(panelCoord.X - MainForm.spaceOuterMm, panelCoord.Y - MainForm.spaceOuterMm), Matrix3x2.CreateRotation(GetExPanelRotAngleRad()));
                float tgtXMm = dCornerTLMm.X + tgtRotated.X;
                float tgtYMm = dCornerTLMm.Y + tgtRotated.Y;
                return new Vector2(tgtXMm, tgtYMm);
            }
            else
            {
                float amountX = Utils.ConvertRange(MainForm.spaceOuterMm, MainForm.spaceOuterMm + (MainForm.holderHolesIndentMm * 2f) + MainForm.workImagePhysWidthMm, 0f, 1f, panelCoord.X);
                float amountY = Utils.ConvertRange(MainForm.spaceOuterMm, MainForm.spaceOuterMm + (MainForm.holderHolesIndentMm * 2f) + MainForm.workImagePhysHeightMm, 0f, 1f, panelCoord.Y);
                Vector2 l1p = Vector2.Lerp(dCornerTLMm, dCornerTRMm, amountX);
                Vector2 l2p = Vector2.Lerp(dCorner3Mm, dCorner4Mm, amountX);
                return Vector2.Lerp(l1p, l2p, amountY);
            }
        }

        private bool PanelCheck()
        {
            if (dCornerTLMm.Length() == 0f || dCornerTRMm.Length() == 0f)
            {
                return false;
            }
            float panelSizeWidthMm = MainForm.workImagePhysWidthMm + (MainForm.spaceOuterMm + MainForm.holderHolesIndentMm) * 2f;
            float panelSizeHeightMm = MainForm.workImagePhysHeightMm + (MainForm.spaceOuterMm + MainForm.holderHolesIndentMm) * 2f;
            Vector2 panelA1 = PanelCoordToWorkAreaCoord(new Vector2(0f, 0f));
            Vector2 panelA2 = PanelCoordToWorkAreaCoord(new Vector2(0f, panelSizeHeightMm));
            Vector2 panelA3 = PanelCoordToWorkAreaCoord(new Vector2(panelSizeWidthMm, panelSizeHeightMm));
            Vector2 panelA4 = PanelCoordToWorkAreaCoord(new Vector2(panelSizeWidthMm, 0f));
            if (panelA1.X < 0 || panelA1.Y < 0 || panelA2.X < 0 || panelA2.Y < 0 || panelA3.X < 0 || panelA3.Y < 0 || panelA4.X < 0 || panelA4.Y < 0)
            {
                return false;
            }
            if (panelA1.X > maxTravelMmX || panelA1.Y > maxTravelMmY || panelA2.X > maxTravelMmX || panelA2.Y > maxTravelMmY || panelA3.X > maxTravelMmX || panelA3.Y > maxTravelMmY || panelA4.X > maxTravelMmX || panelA4.Y > maxTravelMmY)
            {
                return false;
            }
            return true;
        }

        private void DoTransformProcess(bool depanelize)
        {
            try
            {
                // Reset
                shouldCancelOp = false;
                transformProgressPercent = 0;
                transformPending = true;
                SetBusyWithLongTask(true);
                StateChanged.Invoke(this, new EventArgs());

                // Проверка калибровки
                if (GetCamToToolDistX() == 0.0f || GetCamToToolDistY() == 0.0f)
                {
                    throw new Exception("Смещение камера-инструмент не откалибровано.");
                }

                // Получить TL угол
                int res = 0;
                float panelWidthMm = MainForm.workImagePhysWidthMm + (MainForm.spaceOuterMm + MainForm.holderHolesIndentMm) * 2f;
                float panelHeightMm = MainForm.workImagePhysHeightMm + (MainForm.spaceOuterMm + MainForm.holderHolesIndentMm) * 2f;
                Vector2 tlDefaultCamLoc = new Vector2(((maxTravelMmX - panelWidthMm) / 2.0f) + GetCamToToolDistX(), ((maxTravelMmY - panelHeightMm) / 2.0f) + GetCamToToolDistY());
                if (depanelize)
                {
                    MoveToAbsolutePositionAndWaitForCompletion(false, tlDefaultCamLoc.X, tlDefaultCamLoc.Y, toolChangeZMm, -1.0f);
                    res = AskUserForInput("Установите фрезу " + cuttingToolDiameterMm.ToString("0.0") + " мм и убедитесь что панель лежит стороной TOP кверху, проект " + MainForm.gerberFilesDir + " и лежит правильным углом (TopLeft->TopLeft станка).", new List<string> { "Да", "Нет" }, 2, false, false, false);
                    if (res > 0)
                    {
                        throw new Exception("Операция отменена.");
                    }
                } else
                {
                    MoveToAbsolutePositionAndWaitForCompletion(false, tlDefaultCamLoc.X, tlDefaultCamLoc.Y, toolChangeZMm, -1.0f);
                    res = AskUserForInput("Установите сверло любого диаметра и убедитесь что панель лежит стороной TOP кверху, проект " + MainForm.gerberFilesDir + " и лежит правильным углом (TopLeft->TopLeft станка).", new List<string> { "Да", "Нет" }, 2, false, false, false);
                    if (res > 0)
                    {
                        throw new Exception("Операция отменена.");
                    }
                }
                MoveToAbsolutePositionAndWaitForCompletion(false, tlDefaultCamLoc.X, tlDefaultCamLoc.Y, probeZMm, -1.0f);
                res = AskUserForInput("Переместитесь так, чтобы реперное отверстие левого-верхнего угла панели оказалось в центре изображения камеры.", new List<string> { "Готово", "Отмена" }, 2, true, false, false);
                if (res > 0)
                {
                    throw new Exception("Операция отменена.");
                }
                dCornerTLMm = new Vector2(GetAbsXMm() - GetCamToToolDistX(), GetAbsYMm() - GetCamToToolDistY());

                // Получить TR угол
                Vector2 trDefaultCamLoc = new Vector2(GetAbsXMm() + MainForm.workImagePhysWidthMm + MainForm.holderHolesIndentMm + MainForm.spaceOuterMm, GetAbsYMm());
                MoveToAbsolutePositionAndWaitForCompletion(false, trDefaultCamLoc.X, trDefaultCamLoc.Y, probeZMm, -1.0f);
                res = AskUserForInput("Переместитесь так, чтобы реперное отверстие правого-верхнего угла панели оказалось в центре изображения камеры.", new List<string> { "Готово", "Отмена" }, 2, true, false, false);
                if (res > 0)
                {
                    throw new Exception("Операция отменена.");
                }
                dCornerTRMm = new Vector2(GetAbsXMm() - GetCamToToolDistX(), GetAbsYMm() - GetCamToToolDistY());

                // Check
                if (!PanelCheck())
                {
                    throw new Exception("Неверная панель, либо она выходит за рабочее поле.");
                }

                // Перемещаемся в начало панели
                MoveToAbsolutePositionAndWaitForCompletion(false, dCornerTLMm.X, dCornerTLMm.Y, probeZMm, -1.0f);

                // Ставим Z
                res = AskUserForInput("Переместитесь по Z так, чтобы сверло/фреза отступала от заготовки примерно на 1-1.5 мм. Это будет безопасная высота. Рабочая будет на " + epDepthMm.ToString("0.0") + " мм ниже (константа).", new List<string> { "Готово", "Отмена" }, 2, false, true, false);
                if (res > 0)
                {
                    throw new Exception("Операция отменена.");
                }
                float safeZMm = GetAbsZMm();
                float workZMm = GetAbsZMm() - epDepthMm;

                // Perform Op
                if (depanelize)
                {
                    // Cut
                    if (contoursToDepanelizeMm.Count <= 0)
                    {
                        throw new Exception("Нет посчитанных контуров для выполнения депанелизации, проверьте Gerber.");
                    }

                    // Запускаем шпиндель и ждем раскрутки
                    SetSpindleRpm(cuttingRpm);

                    // Цикл
                    int totalPoints = contoursToDepanelizeMm.Sum((e) => e.Length);
                    int processedPoints = 0;
                    for (int i = 0; i < contoursToDepanelizeMm.Count; i++)
                    {
                        for (int j = 0; j < contoursToDepanelizeMm[i].Length; j++)
                        {
                            // Компенсировать криво лежащую на рабочем поле панель
                            Vector2 tgtP = PanelCoordToWorkAreaCoord(new Vector2(contoursToDepanelizeMm[i][j].X, contoursToDepanelizeMm[i][j].Y));

                            // Переместиться в эту точку
                            MoveToAbsolutePositionAndWaitForCompletion((j > 0), tgtP.X, tgtP.Y, null, (j > 0) ? cuttingRateMmPerMin : -1.0f);

                            // Первая?
                            if (j == 0)
                            {
                                // Опустить шпиндель
                                MoveToAbsolutePositionAndWaitForCompletion(true, null, null, workZMm, zSinkRateBigMmPerMin);
                            }

                            // Прогресс и отмена
                            processedPoints++;
                            transformProgressPercent = (int)(((float)processedPoints * 100f) / (float)totalPoints);
                            StateChanged.Invoke(this, new EventArgs());
                            if (shouldCancelOp)
                            {
                                break;
                            }
                        }

                        // Контур завершен, поднять шпиндель
                        MoveToAbsolutePositionAndWaitForCompletion(true, null, null, safeZMm, zRetractRateBigMmPerMin);

                        // Отмена
                        if (shouldCancelOp)
                        {
                            break;
                        }
                    }

                    // Останавливаем шпиндель
                    SetSpindleRpm(0);

                    // Поднимаемся на высоту смены инструмента и центр рабочего поля
                    MoveToAbsolutePositionAndWaitForCompletion(false, null, null, probeZMm, -1.0f);
                    MoveToAbsolutePositionAndWaitForCompletion(false, maxTravelMmX / 2.0f, maxTravelMmY / 2.0f, toolChangeZMm, -1.0f);
                } else
                {
                    // Drill
                    List<List<CHole>> ourHolesByTool = GroupHolesAndSortByMoveDistanceAndDiameter(false, true);
                    int i = 0;
                    int holesDrilled = 0;
                    int holesTotalThisRun = ourHolesByTool.Sum(o => o.Count);
                    while (i < ourHolesByTool.Count)
                    {
                        // Ask To Change Tool
                        MoveToAbsolutePositionAndWaitForCompletion(false, null, null, toolChangeZMm, -1.0f);
                        if (i > 0)
                        {
                            res = AskUserForInput("Если все прошло нормально, поставьте в патрон сверло диаметром " + ourHolesByTool[i].First().diameterMm.ToString("0.0") + " мм и нажмите [готово]. Если предыдущее (" + ourHolesByTool[i - 1].First().diameterMm.ToString("0.0") + " мм) сломалось - поставьте его же новую версию и нажмите [повтор].", new List<string> { "Готово", "Повтор", "Отмена" }, 2, false, false, false);
                            if (res > 1)
                            {
                                throw new Exception("Операция отменена.");
                            }
                            else if (res > 0)
                            {
                                i--;
                            }
                        }
                        else
                        {
                            res = AskUserForInput("Поставьте в патрон сверло диаметром " + ourHolesByTool[i].First().diameterMm.ToString("0.0") + " мм и нажмите [готово].", new List<string> { "Готово", "Отмена" }, 2, false, false, false);
                            if (res > 0)
                            {
                                throw new Exception("Операция отменена.");
                            }
                        }

                        // Tool Installed, Move To More Working Height
                        MoveToAbsolutePositionAndWaitForCompletion(false, null, null, probeZMm, -1.0f);

                        // Start spindle
                        SetSpindleRpm(drillingRpm);

                        // Drill each of holes
                        for (int j = 0; j < ourHolesByTool[i].Count; j++)
                        {
                            // Отверстие
                            CHole thisHole = ourHolesByTool[i][j];

                            // Переместиться
                            Vector2 tgtPos = PanelCoordToWorkAreaCoord(new Vector2(thisHole.holeCenterMmX, thisHole.holeCenterMmY));
                            MoveToAbsolutePositionAndWaitForCompletion(false, tgtPos.X, tgtPos.Y, safeZMm, -1.0f);

                            // Сверло вниз
                            MoveToAbsolutePositionAndWaitForCompletion(true, null, null, workZMm, (thisHole.diameterMm < 0.5f) ? zSinkRateSmallMmPerMin : zSinkRateBigMmPerMin);

                            // Сверло вверх
                            MoveToAbsolutePositionAndWaitForCompletion(true, null, null, safeZMm, (thisHole.diameterMm < 0.5f) ? zRetractRateSmallMmPerMin : zRetractRateBigMmPerMin);

                            // Прогресс
                            holesDrilled++;
                            transformProgressPercent = (int)((((float)holesDrilled) * 100.0f) / ((float)holesTotalThisRun));
                            StateChanged.Invoke(this, new EventArgs());

                            // Отмена
                            if (shouldCancelOp)
                            {
                                break;
                            }
                        }

                        // Stop spindle
                        SetSpindleRpm(0);

                        // Next Tool
                        i++;

                        // Move To Probe Height
                        MoveToAbsolutePositionAndWaitForCompletion(false, null, null, probeZMm, -1.0f);

                        // Ask If Last Tool Broken
                        if (i >= (ourHolesByTool.Count - 1))
                        {
                            res = AskUserForInput("Если это сверло (" + ourHolesByTool[i].First().diameterMm.ToString("0.0") + " мм) сломалось, замените его и нажмите [повтор], в противном случае нажмите [все нормально].", new List<string> { "Все нормально", "Повтор" }, 2, false, false, false);
                            if (res > 0)
                            {
                                i--;
                            }
                        }

                        // Отмена
                        if (shouldCancelOp)
                        {
                            throw new Exception("Операция отменена.");
                        }
                    }

                    // Поднимаемся на высоту смены инструмента и центр рабочего поля
                    MoveToAbsolutePositionAndWaitForCompletion(false, maxTravelMmX / 2.0f, maxTravelMmY / 2.0f, toolChangeZMm, -1.0f);
                }

                // Finale
                transformPending = false;
                SetBusyWithLongTask(false);
                StateChanged.Invoke(this, new EventArgs());
            }
            catch (Exception exc)
            {
                transformPending = false;
                SetBusyWithLongTask(false);
                ErrorOccurred.Invoke(this, new ErrorEventArgs(exc.Message));
            }
        }

        public void StartDrillingProcess(bool drillMainHoles, int newPanelIdx, float alSafeHeightMm, float alWorkpieceThicknessMm)
        {
            if (!IsConnected() || IsFault())
            {
                throw new Exception("Нет связи со станком или он в состоянии Alarm.");
            }
            if (IsBusyWithLongTask() || spindleActive)
            {
                throw new Exception("Станок занят другим процессом или активен шпиндель, действие недоступно.");
            }
            shouldCancelOp = false;
            workThread = new Thread(new ThreadStart(delegate
            {
                DoDrillingProcess(drillMainHoles, newPanelIdx, alSafeHeightMm, alWorkpieceThicknessMm);
            }));
            workThread?.Start();
        }

        public void StartTransformProcess(bool depanelize)
        {
            if (!IsConnected() || IsFault())
            {
                throw new Exception("Нет связи со станком или он в состоянии Alarm.");
            }
            if (IsBusyWithLongTask() || spindleActive)
            {
                throw new Exception("Станок занят другим процессом или активен шпиндель, действие недоступно.");
            }
            shouldCancelOp = false;
            workThread = new Thread(new ThreadStart(delegate
            {
                DoTransformProcess(depanelize);
            }));
            workThread?.Start();
        }

        public bool IsDrillingPending()
        {
            return (workThread?.IsAlive == true && drillingPending);
        }

        public bool IsTransformPending()
        {
            return (workThread?.IsAlive == true && transformPending);
        }
    }
}
