   M S B A M L   `   `   `  ����L  HEngineSimRecorder, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:EngineSimRecorderEngineSimRecorder  5   	   /    )EngineSimRecorder.GeneratorProgressWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:EngineSimRecorder     5   	     � Title$  Generator Progress��5   	   $	��250��6$   $	��400��61   ��5	      �� 5
   
    �� EndPoint$	 1,1&�6    �� 
StartPoint$	 0,0&�6-   .��5      �  � Color$ 	#FF4A4867��6   � 5      $ 	#FF2F2938��6    � Offset$ 1Q�6-   5      .��5      � .+�5   
   �� -   5         ��Name$ BG��5      $��Stretch=�6    $��Stretch=�6>   ���	#3F000000 �� RadiusX$ 25��6k    �� RadiusY$ 25��6x   $��10,10,10,10q�6�   �� 5   
   -   5         ��Name$ Title��5      $��Engine Sim Exporter��6   $��Center=�6=   $��0,25,0,0q�6Z   $	��Top=�6l   
�������$��22(�6�   $
��Bold"�6�   �� 5   
   -   5        	 ��Name$	 Major��5      $��Major Status��6#   $��Center=�67   $��WrapWithOverflow=�6T   $	��350��6t   $��0,0,0,60q�6�   $��Center=�6�   
�������$��22(�6�   
 �� TextAlignment$
 Center=�6�   �� 5   
   -   5        $	 Minor��5      $��Minor Status��6#   $��Center=�67   $��WrapWithOverflow=�6T   $	��350��6t   $��0,30,0,0q�6�   $��Center=�6�   
�������$��22(�6�   $
 Center=�6�   �� 5   
   -   5         ��Name$ 	OK_Button��5      $��OK��6$   $��Center=�6I   $��
0,0,175,25q�6f   $��Bottom=�6z   $��45��6�   $	��150��6�   
���������6�   Z�  � 	Resources 5       �� 
TargetType �� Property �� Value��%^   '��       �� 5      " ��6   .�5      ��  �@��CornerRadius$ 5��65   5      5      ��5      �� 5      $	 1,1&�6&   $	 0,0&�65   .��5      � $ 	#FF4A4867��6#   5      5!      �� 5#   
   -   5        $ Open_Button��5#      $��Open samples folder��6&   $��Center=�6^   $��
175,0,0,25q�6{   $��Bottom=�6�   $��45��6�   $	��150��6�   
���������6�   Z�  5$      ��%^   '��       �� 5%      " ��6   .�5&      ��  �@��CornerRadius$ 5��65   5'      5(      ��5)      �� 5*      $	 1,1&�6&   $	 0,0&�65   .��5+      � $ 	#FF4A4867��6#   5,      5.      5/      50      