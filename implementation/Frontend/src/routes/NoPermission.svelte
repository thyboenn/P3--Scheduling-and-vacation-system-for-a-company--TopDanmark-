<script>
  import Layout from "../components/Layout.svelte";
  import { redirectIfNotAuthenticated } from "../lib/routes";

  redirectIfNotAuthenticated();
</script>

<Layout>
  <div slot="content" class="centered-page-content">
    <h1>Ingen adgang</h1>
    <p>Du prøvede at tilgå en side, som du ikke har adgang til</p>
  </div>
</Layout>
