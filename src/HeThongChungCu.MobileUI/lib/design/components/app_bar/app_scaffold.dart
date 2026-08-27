import 'package:flutter/material.dart';

import 'package:klks_app/design/tokens/colors.dart';

import 'app_top_bar.dart';

class AppScaffold extends StatelessWidget {
  const AppScaffold({
    super.key,
    this.title,
    required this.body,
    this.appBar,
    this.showAppBar = true,
    this.bottomNavigationBar,
    this.floatingActionButton,
    this.floatingActionButtonLocation,
    this.backgroundColor,
    this.resizeToAvoidBottomInset = true,
    this.leading,
    this.actions,
  }) : assert(
         !showAppBar || appBar != null || title != null,
         'AppScaffold: showAppBar=true nhưng không có title lẫn appBar. '
         'Truyền vào title, appBar, hoặc set showAppBar=false.',
       );

  final String? title;
  final Widget body;
  final PreferredSizeWidget? appBar;
  final bool showAppBar;
  final Widget? bottomNavigationBar;
  final Widget? floatingActionButton;
  final FloatingActionButtonLocation? floatingActionButtonLocation;
  final Color? backgroundColor;
  final bool resizeToAvoidBottomInset;
  final Widget? leading;
  final List<Widget>? actions;

  @override
  Widget build(BuildContext context) {
    PreferredSizeWidget? resolvedAppBar;

    if (showAppBar) {
      resolvedAppBar =
          appBar ??
          (title != null
              ? AppTopBar(title: title!, leading: leading, actions: actions)
              : null);
    }

    return Scaffold(
      appBar: resolvedAppBar,
      backgroundColor: backgroundColor ?? AppColors.background,
      body: body,
      bottomNavigationBar: bottomNavigationBar,
      floatingActionButton: floatingActionButton,
      floatingActionButtonLocation: floatingActionButtonLocation,
      resizeToAvoidBottomInset: resizeToAvoidBottomInset,
    );
  }
}
