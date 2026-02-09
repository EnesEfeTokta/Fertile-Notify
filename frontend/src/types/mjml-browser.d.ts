declare module 'mjml-browser' {
  type Mjml2HtmlOptions = Record<string, unknown>;

  interface Mjml2HtmlResult {
    html: string;
    errors?: unknown[];
  }

  const mjml2html: (input: string, options?: Mjml2HtmlOptions) => Mjml2HtmlResult;
  export default mjml2html;
}
