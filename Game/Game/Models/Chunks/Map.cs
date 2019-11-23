﻿using Annex.Graphics;
using Game.Models.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Game.Models.Chunks
{
    public class Map : IDrawableObject
    {
        private Dictionary<(int x, int y), MapChunk> _chunks;
        private HashSet<Entity> _mapEntities;
        public readonly string Name;
        public string Folder => $"resources/maps/{Name}/";
        public string GetChunkFile(int x, int y) => Folder + $"({x})({y}).json";

        public Map(string name) {
            this.Name = name;
            this._chunks = new Dictionary<(int, int), MapChunk>();
            this._mapEntities = new HashSet<Entity>();
        }

        public MapChunk GetChunk(int x, int y) {
            LoadChunk(x, y);
            return this._chunks[(x, y)];
        }

        public void Draw(ICanvas canvas) {
            foreach (var value in this._chunks.Values) {
                value.Draw(canvas);
            }
            foreach (var entity in this.GetOrderedEntities()) {
                entity.Draw(canvas);
            }
        }

        internal void Save() {
            Directory.CreateDirectory(Folder);
            foreach (var entry in this._chunks) {
                Json.SaveChunk(GetChunkFile(entry.Key.x, entry.Key.y), entry.Value);
            }
        }

        public void LoadChunk(int x, int y) {
            if (!this._chunks.ContainsKey((x, y))) {
                this._chunks[(x, y)] = Json.LoadChunk(GetChunkFile(x, y)) ?? new MapChunk(x, y);
            }
        }

        public void AddEntity(Entity e) {
            this._mapEntities.Add(e);
        }

        public IEnumerable<Entity> GetOrderedEntities() {
            return this._mapEntities.OrderBy(e => e.Position.Y);
        }

        public void RemoveEntity(Entity e) {
            this._mapEntities.Remove(e);
        }
    }
}
