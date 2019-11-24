using Annex.Graphics;
using Game.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Models.Chunks
{
    public class Map : IDrawableObject
    {
        private Dictionary<(int, int), MapChunk> _chunks;
        private HashSet<Entity> _mapEntities;

        public Map() {
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

        public void LoadChunk(int x, int y) {
            if (!this._chunks.ContainsKey((x, y))) {
                this._chunks[(x, y)] = new MapChunk(x, y);
            }
        }

        public void AddEntity(Entity e) {
            this._mapEntities.Add(e);
        }

        public IEnumerable<Entity> GetOrderedEntities() {
            return this._mapEntities.OrderBy(e => e.Position.Y);
        }

        public IEnumerable<Entity> GetEntities(Func<Entity, bool> cmp = null)
        {
            if (cmp == null)
            {
                return this._mapEntities;
            }
            return this._mapEntities.Where(cmp);
        }

        public void RemoveEntity(Entity e) {
            this._mapEntities.Remove(e);
        }
    }
}
