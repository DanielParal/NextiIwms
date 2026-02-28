using MediatR;
using Microsoft.Extensions.Logging;
using Nexticz.Module.Sign.Settings.Contracts.DocumentTemplates;
using Nexticz.Module.Sign.Settings.Application.Constants;
using Nexticz.Module.Sign.Settings.Application.Constants.Commands.CreateConstant;
using Nexticz.Module.Sign.Settings.Application.DocumentTemplates.Commands.CreateDocumentTemplate;
using Nexticz.Module.Sign.Settings.Application.EmailTemplates.Commands.CreateEmailTemplate;
using Nexticz.Module.Sign.Settings.Domain.ConstantAggregate;

namespace Nexticz.Module.Sign.Settings.Application.Seeds.Orchestrators;

internal class SeedOrchestrator(
    ISender sender,
    ILogger<SeedOrchestrator> logger) : ISeedOrchestrator
{
    private static readonly List<(string Code, TextOffsetContract[] TextOffsets, TextBackgroundContract[] TextBackgrounds)> DocumentTemplateRequests = 
    [
        new ("RS_YY7_NAKLADNI_LIST_PREP_NEBEZP_VECI_CR_TP",
            [
                new TextOffsetContract("DriverSignature", -80, -37, 0, 25),
                new TextOffsetContract("DriverName", -110, 0, 0, 0),
                new TextOffsetContract("DriverDate", -130, 25, 0, 0),
                new TextOffsetContract("DriverTime", -50, 25, 0, 0),
                
                new TextOffsetContract("UserSignature", -80, -57, 0, 55),
                new TextOffsetContract("UserName", -110, 0, 0, 0),
                new TextOffsetContract("UserDate", -130, 25, 0, 0),
                new TextOffsetContract("UserTime", -50, 25, 0, 0),
                
                new TextOffsetContract("DriverNameDescription", 30, 1, 0, 0),
                new TextOffsetContract("DriverLicensePlateDescription", 35, 2, 0, 0)
            ],
            [
                new TextBackgroundContract("DriverNameDescription", -2, 50),
                new TextBackgroundContract("DriverLicensePlateDescription", -2, 50),
            ]),
        new ("RS_YVZ_VYDEJKA_DODACI_LIST_SARZE",
            [
                new TextOffsetContract("DriverSignature", 0, -15, 0, 33),
                new TextOffsetContract("DriverName", -10, 40, 0, 0),
                new TextOffsetContract("DriverDate", 10, -45, 0, 0),
                
                new TextOffsetContract("UserSignature", -45, -35, 0, 55),
                new TextOffsetContract("UserName", -10, 40, 0, 0),
                new TextOffsetContract("UserDate", 10, -45, 0, 0)
            ],
            []),
        new ("RS_YX4_VYDEJKA_DODACI_LIST_POR",
            [
                new TextOffsetContract("DriverSignature", 0, -15, 0, 33),
                new TextOffsetContract("DriverName", -10, 40, 0, 0),
                new TextOffsetContract("DriverDate", 10, -45, 0, 0),
                
                new TextOffsetContract("UserSignature", -45, -35, 0, 55),
                new TextOffsetContract("UserName", -10, 40, 0, 0),
                new TextOffsetContract("UserDate", 10, -45, 0, 0)
            ],
            []),
        new ("RS_YX4_VYDEJKA_DODACI_LIST_ECOLAB",
            [
                new TextOffsetContract("DriverSignature", 0, -12, 0, 33),
                new TextOffsetContract("DriverName", -10, 48, 0, 0),
                new TextOffsetContract("DriverDate", 10, -32, 0, 0),
                
                new TextOffsetContract("UserSignature", -45, -35, 0, 55),
                new TextOffsetContract("UserName", -10, 45, 0, 0),
                new TextOffsetContract("UserDate", 10, -36, 0, 0)
            ],
            [])
    ];
    
    private static readonly List<(string Code, string Name, string Subject, string HtmlBody, string TextBody)> EmailTemplateRequests = 
    [
        new ("DeliveryDocumentTemplate", "Šablona dodacího listu", "OBJ: {{ORDER_NUMBER}} – Podepsaný dodací list JIPOCAR",
            """
            <!DOCTYPE html>
            <html lang="cs">
            <head>
              <meta charset="UTF-8">
              <title>Dodací list</title>
            </head>
            <body style="font-family: Arial, sans-serif; color: #000000; line-height: 1.4; margin: 0; padding: 0;">
              <table width="100%" cellpadding="0" cellspacing="0" border="0">
                <tr>
                  <td align="center">
                    <table width="600" cellpadding="0" cellspacing="0" border="0" style="border-collapse: collapse;">
                      <tr>
                        <td style="padding: 20px;">
                          <p style="margin: 4px 0;">Dobrý den,</p>
                          <p style="margin: 4px 0;">zasíláme Vám podepsaný dodací list.</p>
            
                          <table width="100%" cellpadding="6" cellspacing="0" border="1" style="border-collapse: collapse; border-color: #ddd;">
                            <tbody>
                              <tr>
                                <td style="background-color: #f2f2f2; padding: 6px 8px;">Ukladatel</td>
                                <td style="padding: 6px 8px; font-weight: bold;">{{DEPOSITOR_CODE}}</td>
                              </tr>
                              <tr>
                                <td style="background-color: #f2f2f2; padding: 6px 8px;">Objednávka</td>
                                <td style="padding: 6px 8px; font-weight: bold;">{{ORDER_NUMBER}}</td>
                              </tr>
                              <tr>
                                <td style="background-color: #f2f2f2; padding: 6px 8px;">Dodací list</td>
                                <td style="padding: 6px 8px; font-weight: bold;">{{DELIVERY_DOCUMENT_CODE}}</td>
                              </tr>
                              <tr>
                                <td style="background-color: #f2f2f2; padding: 6px 8px;">RZNO kód</td>
                                <td style="padding: 6px 8px; font-weight: bold;">{{RZNO_CODE}}</td>
                              </tr>
                              <tr>
                                <td style="background-color: #f2f2f2; padding: 6px 8px;">Sloučený RZNO kód</td>
                                <td style="padding: 6px 8px; font-weight: bold;">{{RZNO_CODE_COMBINED}}</td>
                              </tr>
                              <tr>
                                <td style="background-color: #f2f2f2; padding: 6px 8px;">Odběratel</td>
                                <td style="padding: 6px 8px; font-weight: bold;">{{PARTNER}}</td>
                              </tr>
                              <tr>
                                <td style="background-color: #f2f2f2; padding: 6px 8px;">Příjemce</td>
                                <td style="padding: 6px 8px; font-weight: bold;">{{RECEIVER}}</td>
                              </tr>
                              <tr>
                                <td style="background-color: #f2f2f2; padding: 6px 8px;">Nakládka</td>
                                <td style="padding: 6px 8px; font-weight: bold;">{{LOADING_DOCUMENT_CODE}}</td>
                              </tr>
                              <tr>
                                <td style="background-color: #f2f2f2; padding: 6px 8px;">Datum nakládky</td>
                                <td style="padding: 6px 8px; font-weight: bold;">{{LOADING_DATE}}</td>
                              </tr>
                              <tr>
                                <td style="background-color: #f2f2f2; padding: 6px 8px;">Jméno řidiče</td>
                                <td style="padding: 6px 8px; font-weight: bold;">{{DRIVER_NAME}}</td>
                              </tr>
                              <tr>
                                <td style="background-color: #f2f2f2; padding: 6px 8px;">SPZ</td>
                                <td style="padding: 6px 8px; font-weight: bold;">{{LICENSE_PLATE}}</td>
                              </tr>
                              <tr>
                                <td style="background-color: #f2f2f2; padding: 6px 8px;">Hmotnost zásilky [kg]</td>
                                <td style="padding: 6px 8px; font-weight: bold;">{{WEIGHT}}</td>
                              </tr>
                              <tr>
                                <td style="background-color: #f2f2f2; padding: 6px 8px;">ADR</td>
                                <td style="padding: 6px 8px; font-weight: bold;">{{ADR_POINTS}}</td>
                              </tr>
                            </tbody>
                          </table>
            
                          <p style="margin: 4px 0;">Tento e-mail je generován automaticky. Prosíme, neodpovídejte na něj.</p>
                          <p style="margin: 4px 0;">Děkujeme<br>Team JIPOCAR</p>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>
            </body>
            </html>
            """,
            """
            Dobrý den,
            zasíláme Vám podepsaný dodací list.
            
            Ukladatel: {{DEPOSITOR_CODE}}
            Objednávka: {{ORDER_NUMBER}}
            Dodací list: {{DELIVERY_DOCUMENT_CODE}}
            RZNO kód: {{RZNO_CODE}}
            Sloučený RZNO kód: {{RZNO_CODE_COMBINED}}
            Odběratel: {{PARTNER}}
            Příjemce: {{RECEIVER}}
            Nakládka: {{LOADING_DOCUMENT_CODE}}
            Datum nakládky: {{LOADING_DATE}}
            Jméno řidiče: {{DRIVER_NAME}}
            SPZ: {{LICENSE_PLATE}}
            Hmotnost zásilky [kg]: {{WEIGHT}}
            ADR: {{ADR_POINTS}}
            
            Tento e-mail je generován automaticky. Prosíme, neodpovídejte na něj.
            Děkujeme
            Team JIPOCAR
            """
            ),
        new ("LoadingDocumentTemplate", "Šablona nakládkového listu", "NL: {{LOADING_DOCUMENT_CODE}} – Podepsané přepravní dokumenty JIPOCAR",
            """
            <!DOCTYPE html>
            <html lang="cs">
              <head>
                <meta charset="UTF-8">
                <title>Přepravní dokumenty</title>
              </head>
              <body style="margin:0; padding:0; font-family: Arial, sans-serif; color:#000000; line-height:1.4;">
                <table width="100%" cellpadding="0" cellspacing="0" border="0">
                  <tr>
                    <td align="center">
                      <!-- Outer container -->
                      <table width="700" cellpadding="0" cellspacing="0" border="0" style="border-collapse:collapse;">
                        <tr>
                          <td style="padding:20px;">
                            
                            <p style="margin:4px 0;">Dobrý den,</p>
                            <p style="margin:4px 0;">zasíláme Vám podepsané přepravní dokumenty.</p>
            
                            <p style="margin:4px 0;">
                              Ukladatel: <strong>{{DEPOSITOR_CODE}}</strong><br>
                              Nakládka: <strong>{{LOADING_DOCUMENT_CODE}}</strong><br>
                              Datum nakládky: <strong>{{LOADING_DATE}}</strong><br>
                              Jméno řidiče: <strong>{{DRIVER_NAME}}</strong><br>
                              SPZ: <strong>{{LICENSE_PLATE}}</strong><br>
                              Celková hmotnost [kg]: <strong>{{TOTAL_WEIGHT}}</strong><br>
                              ADR celkem: <strong>{{ADR_POINTS}}</strong>
                            </p>
            
                            <p style="margin:10px 0 4px 0;">Dodací listy:</p>
            
                            <!-- Table -->
                            <table width="100%" cellpadding="6" cellspacing="0" border="1" style="border-collapse:collapse; border-color:#ddd;">
                              <thead>
                                <tr>
                                  <th style="background-color:#f2f2f2; padding:6px 8px; text-align:left;">Objednávka</th>
                                  <th style="background-color:#f2f2f2; padding:6px 8px; text-align:left;">Dodací list</th>
                                  <th style="background-color:#f2f2f2; padding:6px 8px; text-align:left;">RZNO</th>
                                  <th style="background-color:#f2f2f2; padding:6px 8px; text-align:left;">Sl. RZNO</th>
                                  <th style="background-color:#f2f2f2; padding:6px 8px; text-align:left;">Odběratel</th>
                                  <th style="background-color:#f2f2f2; padding:6px 8px; text-align:left;">Příjemce</th>
                                  <th style="background-color:#f2f2f2; padding:6px 8px; text-align:left;">Hmotnost</th>
                                  <th style="background-color:#f2f2f2; padding:6px 8px; text-align:left;">ADR</th>
                                </tr>
                              </thead>
                              <tbody>
                                {{#DELIVERY_DOCUMENTS}}
                                <tr>
                                  <td style="padding:6px 8px;">{{LOOP_ORDER_NUMBER}}</td>
                                  <td style="padding:6px 8px;">{{LOOP_DELIVERY_DOCUMENT_CODE}}</td>
                                  <td style="padding:6px 8px;">{{LOOP_RZNO_CODE}}</td>
                                  <td style="padding:6px 8px;">{{LOOP_RZNO_CODE_COMBINED}}</td>
                                  <td style="padding:6px 8px;">{{LOOP_PARTNER}}</td>
                                  <td style="padding:6px 8px;">{{LOOP_RECEIVER}}</td>
                                  <td style="padding:6px 8px;">{{LOOP_WEIGHT}}</td>
                                  <td style="padding:6px 8px;">{{LOOP_ADR_POINTS}}</td>
                                </tr>
                                {{/DELIVERY_DOCUMENTS}}
                              </tbody>
                            </table>
            
                            <p style="margin:10px 0 4px 0;">
                              Tento e-mail je generován automaticky. Prosíme, neodpovídejte na něj.<br>
                              Děkujeme<br>
                              Team JIPOCAR
                            </p>
            
                          </td>
                        </tr>
                      </table>
                      <!-- End container -->
                    </td>
                  </tr>
                </table>
              </body>
            </html>
            """,
            """
            Dobrý den,
            zasíláme Vám podepsané přepravní dokumenty.
            
            Ukladatel: {{DEPOSITOR_CODE}}
            Nakládka: {{LOADING_DOCUMENT_CODE}}
            Datum nakládky: {{LOADING_DATE}}
            Jméno řidiče: {{DRIVER_NAME}}
            SPZ: {{LICENSE_PLATE}}
            Celková hmotnost [kg]: {{TOTAL_WEIGHT}}
            ADR celkem: {{ADR_POINTS}}
            
            Dodací listy:
            
            Objednávka | Dodací list | RZNO   | Sl. RZNO | Odběratel           | Příjemce      | Hmotnost | ADR
            --------------------------------------------------------------------------------------------------------
            {{DELIVERY_DOCUMENTS_LOOP}}
            
            Tento e-mail je generován automaticky. Prosíme, neodpovídejte na něj.
            Děkujeme
            Team JIPOCAR
            """)
    ];

    private static readonly List<(string Key, string Value, Domain.ConstantAggregate.ConstantType ConstantType, string Description)> ConstantRequests =
    [
      new (ConstantNames.EmailsForMissingRequiredDataDuringTransfer, "", ConstantType.String, "Emailové adresy oddělené středníkem pro kontrolu chybějícího ukladatele nebo způsobu dodání v systému."),
    ];
    
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        // Document templates
        foreach (var documentTemplateRequest in DocumentTemplateRequests)
        {
            await sender.Send(new CreateDocumentTemplateCommand(
                documentTemplateRequest.Code, documentTemplateRequest.TextOffsets, documentTemplateRequest.TextBackgrounds), 
                cancellationToken);
        }
        
        // Email templates
        foreach (var emailTemplateRequest in EmailTemplateRequests)
        {
            await sender.Send(new CreateEmailTemplateCommand(
                emailTemplateRequest.Code, emailTemplateRequest.Name, emailTemplateRequest.Subject, emailTemplateRequest.HtmlBody, emailTemplateRequest.TextBody), 
                cancellationToken);
        }
        
        // Constants
        foreach (var constantRequest in ConstantRequests)
        {
            await sender.Send(
              new CreateConstantCommand(constantRequest.Key, constantRequest.Value, constantRequest.ConstantType,
                constantRequest.Description), cancellationToken);
        }
        
        logger.LogInformation("Sign - Settings - Seeding completed");
    }
}