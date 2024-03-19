<script lang="ts">
  import { Button, Card, Checkbox, Divider, RadioButton } from "attractions";
  import { ChevronDownIcon, ChevronUpIcon } from "svelte-feather-icons";
  import { permissionTranslations } from "../../lib/permissions";
  import { makeBackendRequest } from "../../lib/requestHelpers";
  import type { AssociateDTO, PermissionType, RoleDTO } from "../../lib/types";

  export let associate: AssociateDTO;
  export let roles: Omit<RoleDTO, "permissions">[];

  let editingAssociate:
    | {
        extraPermissions: Set<PermissionType>;
        roleId: string | null;
        color: string | null;
        touched: boolean;
      }
    | undefined = undefined;

  function onPermissionChange(
    e: CustomEvent<{ value: string; checked: boolean; nativeEvent: Event }>
  ) {
    if (!editingAssociate) return;

    const { value, checked } = e.detail;
    const permissionValue = value as PermissionType;

    checked
      ? editingAssociate.extraPermissions.add(permissionValue)
      : editingAssociate.extraPermissions.delete(permissionValue);

    editingAssociate.touched = true;
  }

  async function saveChanges() {
    if (!editingAssociate) return;

    const promises = [];

    // Remove previous permissions
    for (let permission of associate.extraPermissions) {
      if (editingAssociate.extraPermissions.has(permission)) continue;

      promises.push(
        makeBackendRequest("/Associate/permission/remove", "delete", {
          associateId: associate.id,
          permission,
        })
      );
    }

    // Add new permissions
    for (let permission of editingAssociate.extraPermissions) {
      if (associate.extraPermissions.includes(permission)) continue;

      promises.push(
        makeBackendRequest("/Associate/permission/add", "post", {
          associateId: associate.id,
          permission,
        })
      );
    }

    // Change role
    if (
      editingAssociate.roleId != null &&
      editingAssociate.roleId !== associate?.roleId
    ) {
      promises.push(
        makeBackendRequest("/Associate/role", "post", {
          associateId: associate.id,
          roleId: editingAssociate.roleId,
        })
      );
    }

    // Change color
    if (editingAssociate.color && editingAssociate.color !== associate.color) {
      promises.push(
        makeBackendRequest("/Associate/color", "post", {
          associateId: associate.id,
          ...hexToRgb(editingAssociate.color),
        })
      );
    }

    await Promise.all(promises);

    associate.extraPermissions = [...editingAssociate.extraPermissions];
    associate.roleId = editingAssociate.roleId ?? null;
    associate.color = editingAssociate.color;

    editingAssociate.touched = false;
  }

  function hexToRgb(hex: string): { r: number; g: number; b: number } {
    const rString = hex.slice(1, 3);
    const gString = hex.slice(3, 5);
    const bString = hex.slice(5, 7);

    return {
      r: parseInt(rString, 16),
      g: parseInt(gString, 16),
      b: parseInt(bString, 16),
    };
  }
</script>

<Card>
  <h2>{associate.name}</h2>

  {#if associate.roleId}
    <h3>{roles.find((r) => r.id === associate.roleId)?.name ?? ""}</h3>
  {/if}
  <Button
    small
    outline
    round
    on:click={() =>
      (editingAssociate = editingAssociate
        ? undefined
        : {
            extraPermissions: new Set(associate.extraPermissions),
            roleId: associate?.roleId,
            color: associate.color,
            touched: false,
          })}
  >
    {#if editingAssociate}<ChevronDownIcon />{:else}<ChevronUpIcon />{/if}
  </Button>

  {#if editingAssociate != undefined}
    <Divider />
    <h4>Rolle</h4>
    <div class="spaced">
      {#each roles as role}
        <RadioButton
          name="roles"
          value={role.id}
          title={role.name}
          bind:group={editingAssociate.roleId}
          on:change={() => editingAssociate && (editingAssociate.touched = true)}
        >
          <span class="ml">{role.name}</span>
        </RadioButton>
      {/each}
    </div>

    <h4>Tilladelser</h4>
    <div class="spaced">
      {#each Object.entries(permissionTranslations) as [value, title]}
        <Checkbox
          {value}
          {title}
          checked={associate.extraPermissions?.some(
            (permission) => permission === value
          )}
          on:change={onPermissionChange}
        >
          <span class="ml">{title}</span>
        </Checkbox>
      {/each}
    </div>

    <h4>Farve</h4>
    <input
      type="color"
      bind:value={editingAssociate.color}
      on:change={() => editingAssociate && (editingAssociate.touched = true)}
    />

    {#if editingAssociate.touched}
      <div class="row space-between" style="margin-top: 2em;">
        <Button filled on:click={saveChanges}>Gem</Button>
        <Button danger outline on:click={() => (editingAssociate = undefined)}>
          Afbryd
        </Button>
      </div>
    {/if}
  {/if}
</Card>
