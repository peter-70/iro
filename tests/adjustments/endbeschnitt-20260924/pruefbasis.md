# Prüfbasis der begrenzten Endbeschnittfreigabe

Stand: 24. September 2026; Analyse 0.5.9. SHA256 der geprüften Quelldateien.

129 gezielte Kernprüfungen, 25 Integrationsprüfungen und der anschließende Zwei-Bilder-Gegencheck bestanden. Android-Release-Build: 0 Fehler, 8 bekannte XC0022-Warnungen. Keine ausdrückliche Abnahme, kein Schluss-Sammellauf und kein Gerätetest.

| Datei | SHA256 |
| --- | --- |
| src/iro.core/Analysis/ImageAnalyzer.cs | 41EB98E98E66C437BAC85F72608C3D9AE20B6F8BE5F290E9D1D3891BF78F19B4 |
| src/iro.core/Analysis/MeasurementSafety.cs | E301DD1CB30825EA59942F4941E6C265486151375B300211C935A8DB3DBB8F24 |
| src/iro.core/Analysis/StripDetector.cs | CBA0D619291D8C2D529CBC98819A7501759366B8ABEA4CA9BB3CADAB3BBBE8CB |
| tests/iro.core.tests/CropAcceptanceTests.cs | D2A9067302B0F175CE41ADB8D9E6CBCD246FCF3DED5C2E814C75B20B3F0ECEDD |
| iro-gen/TestRunReview.cs | 22018D930DA65085DF987951EECB2C84238FAC8111F36D43222EBA246EAD9AD8 |
| tests/iro.gen.tests/ReviewDiagnosticsTests.cs | C8A887659FD8E810CD31B922444D08D6EB7C1DD2A667AF492518C102963B6291 |
| tests/iro.gen.tests/AnalysisIntegrationTests.cs | B1CDF6DC883BD1D564DEC15C84743BB6918B94A677C17CB450AD139770FB23FA |
