using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimController:MonoBehaviour {



	public Animation unity_animation ;

	public string anim_asset_path;

	public Dictionary<string,string> loaded_anims;

	public string current_anim_name;
	public WrapMode wrap_mode;
	public bool localRewind;
	public int play_count = 0;

	public void Init(Animation unity_animation, string anim_asset_path){
		this.unity_animation = unity_animation;
		var ss_anim_asset_path_pro = anim_asset_path;
		this.anim_asset_path = ss_anim_asset_path_pro;

		this.loaded_anims = new Dictionary<string,string> ();
	}

	public int getCurrentPlayCount(){
		return this.play_count;
	}

	public int playAnim(string anim_name,WrapMode wrap_mode ,bool rewind)
	{
		this.current_anim_name = anim_name;
		this.wrap_mode = wrap_mode;
		this.localRewind = rewind;
		if (anim_name == "") {
			return play_count;
		}

		if (this.unity_animation.GetClip(anim_name) ) {
				this.palyAnim0 (anim_name, wrap_mode, rewind);
		}
		else if (this.loaded_anims.ContainsKey(anim_name) ){

		}
		else{
			this.loaded_anims [anim_name] = anim_name;

			string res_path = this.anim_asset_path+"/"+anim_name;

			ResourceManager.Instance.LoadResourceAsync (res_path,
				delegate(string str, Object asset) {
					
					if (asset == null) {
						return;
					}


					this.unity_animation.AddClip (asset as AnimationClip, anim_name);

					if (this.current_anim_name == anim_name) {
						this.palyAnim0 (anim_name, wrap_mode, rewind);
					}
				}
			);
		}

		this.play_count++;
		return play_count;
	}

	private void palyAnim0(string anim_name,WrapMode wrap_mode ,bool rewind){


		if (this.unity_animation == null) {
			Debug.LogError ("..unity_animation nil");
		}

		//这句代码有bug
		//this.unity_animation.GetClip (anim_name).wrapMode = wrapMode;

		this.unity_animation[anim_name].wrapMode = wrap_mode;

			if (this.unity_animation.IsPlaying(anim_name)) {
					if (rewind) {
						this.unity_animation.Rewind(anim_name);
						this.unity_animation.Play(anim_name);
					}


				}
			else{
				this.unity_animation.Play(anim_name);
			}
	}
				public void unLoadResource(){}


//	public bool isPlaying = false;
//	void Update(){
//		if (this.unity_animation != null && !string.IsNullOrEmpty(this.current_anim_name)) {
//			this.isPlaying = this.unity_animation.IsPlaying (this.current_anim_name);
//		}
//	}
}
