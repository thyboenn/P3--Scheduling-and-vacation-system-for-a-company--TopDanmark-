<script lang="ts">
  import { Loading } from "attractions";
  import AssociateCard from "../components/Associates/AssociateCard.svelte";
  import Layout from "../components/Layout.svelte";
  import { redirectWithoutPermission } from "../lib/permissions";
  import { redirectIfNotAuthenticated } from "../lib/routes";
  import { associates, me, updateAssociates } from "../stores/associate";
  import { ensureRoles, roles } from "../stores/roles";

  redirectIfNotAuthenticated();
  redirectWithoutPermission($me, ["CanManageAssociates"]);

  updateAssociates();
  ensureRoles();
</script>

<Layout>
  <div slot="content" class="centered-page-content">
    <h1>Ansatte</h1>
    {#if $associates && $roles}
      <div class="spaced">
        {#each $associates as associate}
          <AssociateCard {associate} roles={$roles} />
        {/each}
      </div>
    {:else}
      <Loading />
    {/if}
  </div>
</Layout>
