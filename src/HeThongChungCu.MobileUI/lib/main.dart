import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import 'core/storage/user_session.dart';
import 'core/navigation/app_router.dart';

import 'design/design.dart';

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();

  await SystemChrome.setPreferredOrientations([DeviceOrientation.portraitUp]);

  await UserSession.instance.load();

  runApp(const App());
}

class App extends StatelessWidget {
  const App({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp.router(
      title: AppConstants.appName,
      debugShowCheckedModeBanner: false,
      routerConfig: AppRouter.router,
      theme: AppTheme.light,
      themeMode: ThemeMode.light,
    );
  }
}
