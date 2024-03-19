<script lang="ts">
  import { Button, Card, Divider, Loading, TextField } from "attractions";
  import { onMount } from "svelte";
  import { PlusIcon } from "svelte-feather-icons";
  import Layout from "../components/Layout.svelte";
  import RoleCard from "../components/Roles/RoleCard.svelte";
  import { redirectWithoutPermission } from "../lib/permissions";
  import { makeBackendRequestJson } from "../lib/requestHelpers";
  import { redirectIfNotAuthenticated } from "../lib/routes";
  import type { CreateRoleDTO } from "../lib/types";
  import { me } from "../stores/associate";
  import { ensureRoles, roles, updateRoles } from "../stores/roles";

  onMount(() => {
    redirectIfNotAuthenticated();
    redirectWithoutPermission($me, ["CanManageAssociates"]);
    ensureRoles();
  });

  let createRoleDto: CreateRoleDTO | undefined;

  async function createRole() {
    if (createRoleDto) {
      await makeBackendRequestJson("/Role", "post", createRoleDto);
      updateRoles();
      createRoleDto = undefined;
    }
  }
</script>

<Layout>
  <div slot="content" class="centered-page-content spaced">
    <h1>Roller</h1>
    {#if $roles}
      {#if $roles.length === 0}
        <p>Der er ingen roller endnu.</p>
      {:else}
        {#each $roles as role}
          <RoleCard {role} />
        {/each}
      {/if}
      {#if !createRoleDto}
        <Button
          style="margin-left: auto; margin-right: auto;"
          on:click={() => (createRoleDto = { name: "" })}
        >
          <PlusIcon />
        </Button>
      {:else}
        <Divider />
        <div>
          <h2>Opret ny rolle</h2>
          <Card class="spaced">
            <TextField
              outline
              withItem
              label="Role Name"
              bind:value={createRoleDto.name}
            />
            <div class="row space-between">
              <Button filled on:click={createRole}>Opret rolle</Button>
              <Button danger outline on:click={() => (createRoleDto = undefined)}>
                Afbryd
              </Button>
            </div>
          </Card>
        </div>
      {/if}
    {:else}
      <Loading />
    {/if}
  </div>
</Layout>
