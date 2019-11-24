using Annex.Events;
using Annex.Graphics;
using Annex.Scenes;
using Annex.Scenes.Components;
using Game.Models.Chunks;
using System.Collections.Generic;

namespace Game.Scenes
{
    public abstract class SceneWithMap : Scene
    {
        public static SceneWithMap CurrentScene => SceneManager.Singleton.CurrentScene as SceneWithMap;

        public readonly Map map;
        protected List<ScrollingTextMessage> _scrollingTexts;

        public SceneWithMap(string mapName) {
            this._scrollingTexts = new List<ScrollingTextMessage>();
            this.map = new Map(mapName);

            this.Events.AddEvent("", PriorityType.ANIMATION, () => {
                for (int i = 0; i < this._scrollingTexts.Count; i++) {
                    if (this._scrollingTexts[i].Update()) {
                        this._scrollingTexts.RemoveAt(i--);
                    }
                }
                return ControlEvent.NONE;
            }, 10);
        }

        public void AddScrollingMessage(ScrollingTextMessage msg) {
            this._scrollingTexts.Add(msg);
        }

        public override void Draw(ICanvas canvas) {
            this.map.Draw(canvas);
            for (int i = 0; i < this._scrollingTexts.Count; i++) {
                this._scrollingTexts[i].Draw(canvas);
            }
            base.Draw(canvas);
        }
    }
}
