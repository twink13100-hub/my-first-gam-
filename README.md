# CrashDrive 3D — Unity Android starter

This is a new Unity 6 starter project. It creates a simple 3D driving test area and car at runtime, with WheelCollider physics, touch controls, camera follow, speed display, and reset.

## Build an APK

1. Upload the **contents** of this folder to a GitHub repository.
2. Configure a valid Unity license for GitHub Actions using the GameCI activation guide: https://game.ci/docs/github/activation/
3. Add the required repository secret(s), normally `UNITY_LICENSE`; some license types also need `UNITY_EMAIL` and `UNITY_PASSWORD`.
4. Open Actions → **Build CrashDrive 3D Android APK** → Run workflow.
5. Download `CrashDrive3D-Android-APK` from the successful run.

This archive does not contain a prebuilt APK. I have not run Unity here, so the first cloud build is the real verification step. The workflow will fail rather than upload a fake artifact if Unity cannot build the APK.

Unity version: 6000.0.32f1. This is a basic prototype, not BeamNG.drive; it does not include soft-body deformation or advanced damage physics.
