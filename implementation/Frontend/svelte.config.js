import makeAttractionsImporter from "attractions/importer.js";
import path from "path";
import sveltePreprocess from "svelte-preprocess";

const __dirname = path.resolve();

export default {
  preprocess: [
    sveltePreprocess({
      scss: {
        importer: makeAttractionsImporter({
          themeFile: path.join(__dirname, "theme.scss"),
        }),
      },
    }),
  ],
};
