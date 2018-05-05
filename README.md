# Secret_Base_Firebase

YOU WILL NEED TO FOLLOW THESE INSTRUCITONS TO SET UP FIREBASE
https://firebase.google.com/docs/unity/setup

USE THESE TO START IF NEEDED:
https://github.com/firebase/quickstart-unity

I think for most people, the files you should be looking at are UAuth and Dbase. I cut a lot of corners in my code for the sake of time, but an actually good implementation is done in the firebase quickstart unity files linked above. 

I couldn't find it anywhere on the tutorials, but note that you need both the andriod and the ios files that firebase gives you to download, even if you're just building to one of them. Also you need to put the andriod ones into a Streaming-Assets folder. For ios you need a working version of Cocoabeans.

Sorry the code is so Gross. I think there are 100000% more efficient ways to do what I did, but I didn't really have time to optimize it, and it ran normally anyways.

I had some issues with firebase, but I documented the fixes here: http://cmuems.com/2018/60212s/conye/04/06/conye-artech/

Good luck! :)
