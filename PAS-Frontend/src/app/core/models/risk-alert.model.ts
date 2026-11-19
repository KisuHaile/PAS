export type RiskSeverity = 'Low' | 'Medium' | 'High' | 'Critical';
export type RiskAlertStatus = 'New' | 'Acknowledged' | 'Resolved';

export interface RiskAlert {
  id: string;
  title: string;
  description: string;
  severity: RiskSeverity;
  relatedType: string;
  relatedId?: string;
  relatedLabel?: string;
  detectedAt: Date | string;
  status: RiskAlertStatus;
}
