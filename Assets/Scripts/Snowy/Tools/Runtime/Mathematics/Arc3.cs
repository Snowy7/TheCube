﻿using System;
using Snowy.Engine;
using UnityEngine;

namespace Snowy.Mathematics
{
    [Serializable]
    public struct Arc3
    {
        /// <summary>
        /// Start launch position.
        /// </summary>
        public Vector3 StartPos;

        /// <summary>
        /// Start launch horizontal angle in degrees.
        /// </summary>
        public float HorAngle;

        /// <summary>
        /// Start launch vertical angle in degrees.
        /// </summary>
        public float VertAngle;

        /// <summary>
        /// Start launch speed.
        /// </summary>
        public float StartSpeed;

        /// <summary>
        /// Gravity.
        /// </summary>
        public float Gravity;

        /// <summary>
        /// Start launch direction (normalized).
        /// </summary>
        public Vector3 StartDir
        {
            get => AngleToDir(HorAngle, VertAngle);
            set => DirToAngle(value, out HorAngle, out VertAngle);
        }

        /// <summary>
        /// Start launch direction vector with magnitude equals to StartSpeed.
        /// </summary>
        public Vector3 StartVector
        {
            get => AngleToDir(HorAngle, VertAngle) * StartSpeed;
            set
            {
                DirToAngle(value, out HorAngle, out VertAngle);
                StartSpeed = value.magnitude;
            }
        }

        public Arc3(float vertAngle, float horAngle, float startSpeed, float gravity, in Vector3 startPos = default)
        {
            VertAngle = vertAngle;
            HorAngle = horAngle;
            StartSpeed = startSpeed;
            Gravity = gravity;
            StartPos = startPos;
        }

        public Arc3(Vector3 dir, float startSpeed, float gravity, in Vector3 startPos = default)
        {
            DirToAngle(dir, out HorAngle, out VertAngle);
            StartSpeed = startSpeed;
            Gravity = gravity;
            StartPos = startPos;
        }

        public Vector3 Evaluate(float time)
        {
            Vector3 newPos = Arc2.GetArcPos(VertAngle, StartSpeed, Gravity, time);
            return newPos.GetRotated(Vector3.up, HorAngle) + StartPos;
        }

        private static Vector3 AngleToDir(float hor, float vert)
        {
            return Quaternion.Euler(0f, hor, vert) * Vector3.right;
        }

        private static void DirToAngle(Vector3 dir, out float hor, out float vert)
        {
            Vector2 startDir2D = dir.XZ().normalized;

            float horAngle = Vector2.Angle(startDir2D, Vector2.right);
            hor = startDir2D.y > 0f ? -horAngle : horAngle;

            Vector3 floorProj = startDir2D.To_XyZ();

            float vertAngle = startDir2D.magnitude <= MathUtility.kEpsilon ? 90f : Vector3.Angle(floorProj, dir);
            vert = dir.y < 0f ? -vertAngle : vertAngle;
        }
    }
}
