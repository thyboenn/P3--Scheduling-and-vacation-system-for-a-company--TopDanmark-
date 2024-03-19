<script lang="ts">
  import { Button, Card, Checkbox, Divider } from "attractions";
  import { ChevronDownIcon, ChevronUpIcon } from "svelte-feather-icons";
  import { permissionTranslations } from "../../lib/permissions";
  import { makeBackendRequestJson } from "../../lib/requestHelpers";
  import type { PermissionType, RoleDTO } from "../../lib/types";
  import { updateRoleInStore } from "../../stores/roles";

  export let role: RoleDTO;

  let editingRole: { data: RoleDTO; touched: boolean } | undefined = undefined;

  function togglePermission(value: string) {
    return () => {
      if (!editingRole) return;

      const beforeLength = editingRole.data.permissions.length;

      editingRole.data.permissions = editingRole.data.permissions.filter(
        (permission) => permission !== value
      );

      if (beforeLength === editingRole.data.permissions.length) {
        editingRole.data.permissions.push(value as PermissionType);
      }

      editingRole.touched = true;
    };
  }

  async function saveRole() {
    if (editingRole) {
      const updatedRole = await makeBackendRequestJson(
        `/Role/edit`,
        "put",
        editingRole.data
      );
      if (updatedRole) {
        role = updatedRole;
        updateRoleInStore(role);
        editingRole.touched = false;
      }
    }
  }
</script>

<Card>
  <h2>{role.name}</h2>
  <Button
    small
    outline
    round
    on:click={() =>
      (editingRole = editingRole
        ? undefined
        : { data: { ...role }, touched: false })}
  >
    {#if editingRole}<ChevronUpIcon />{:else}<ChevronDownIcon />{/if}
  </Button>

  {#if editingRole}
    <Divider />
    <h3>Tilladelser</h3>
    <div class="spaced">
      {#each Object.entries(permissionTranslations) as [value, title]}
        <Checkbox
          {value}
          {title}
          checked={editingRole.data.permissions.some(
            (permission) => permission === value
          )}
          on:change={togglePermission(value)}
        >
          <span class="ml">{title}</span>
        </Checkbox>
      {/each}
      {#if editingRole.touched}
        <div class="row space-between">
          <Button filled on:click={saveRole}>Gem</Button>
          <Button danger outline on:click={() => (editingRole = undefined)}>
            Afbryd
          </Button>
        </div>
      {/if}
    </div>
  {/if}
</Card>
